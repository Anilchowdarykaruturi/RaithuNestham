import {
  Component,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import {
  AiService,
  AIChatResponse
} from '../../services/ai';

import { AuthService } from '../../services/auth';

import {
  DomSanitizer,
  SafeHtml
} from '@angular/platform-browser';

interface ChatMessage {
  sender: 'user' | 'ai';
  text: string;
  html?: SafeHtml;
}

@Component({
  selector: 'app-ai-chat',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './ai-chat.html',
  styleUrl: './ai-chat.css'
})
export class AiChat {
  private aiService = inject(AiService);
  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);
  private sanitizer = inject(DomSanitizer);

  question = '';
  loading = false;
  error = '';

  language: 'en' | 'te' = 'en';

  messages: ChatMessage[] = [
    {
      sender: 'ai',
      text: 'Welcome, Farmer! Ask me about your crops, weather, fertilizers, or pest problems. I am here to help.',
      html: 'Welcome, Farmer! Ask me about your crops, weather, fertilizers, or pest problems. I am here to help.'
    }
  ];

  setLanguage(language: 'en' | 'te'): void {
    this.language = language;

    const welcomeMessage =
      language === 'te'
        ? 'రైతు అన్నా, స్వాగతం! మీ పంటలు, వాతావరణం, ఎరువులు లేదా పురుగుల సమస్యల గురించి అడగండి.'
        : 'Welcome, Farmer! Ask me about your crops, weather, fertilizers, or pest problems. I am here to help.';

    // Update the initial greeting if the conversation has not started.
    if (
      this.messages.length === 1 &&
      this.messages[0].sender === 'ai'
    ) {
      this.messages[0] = {
        sender: 'ai',
        text: welcomeMessage,
        html: welcomeMessage
      };
    }

    this.cdr.detectChanges();
  }

  askQuestion(): void {
    const questionText = this.question.trim();

    if (this.loading) {
      return;
    }

    if (!questionText) {
      this.error = 'Please enter a farming question.';
      return;
    }

    const farmerId = this.authService.getFarmerId();

    if (!farmerId || farmerId <= 0) {
      this.error =
        'Your farmer profile could not be identified. Please log out and log in again.';
      return;
    }

    this.error = '';

    this.messages.push({
      sender: 'user',
      text: questionText
    });

    this.question = '';
    this.loading = true;

    this.aiService.chat({
      farmerId,
      question: questionText
    }).subscribe({
      next: (response: AIChatResponse) => {
        const answer = response?.answer ?? '';

        this.messages.push({
          sender: 'ai',
          text: answer,
          html: this.formatAiResponse(answer)
        });

        this.loading = false;
        this.cdr.detectChanges();
      },

      error: (err: any) => {
        this.error =
          err?.userMessage ||
          err?.error?.message ||
          'Unable to get a response from AI. Please try again.';

        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onEnter(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.askQuestion();
    }
  }

  private formatAiResponse(response: string): SafeHtml {
    // Escape HTML before applying limited formatting.
    let html = response
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');

    // Support Markdown bold.
    html = html.replace(
      /\*\*(.*?)\*\*/g,
      '<strong>$1</strong>'
    );

    // Support bullet points.
    html = html.replace(/^\s*[-*]\s+/gm, '• ');

    // Support horizontal rules.
    html = html.replace(/^\s*---+\s*$/gm, '<hr>');

    // Preserve line breaks.
    html = html.replace(/\r?\n/g, '<br>');

    return this.sanitizer.bypassSecurityTrustHtml(html);
  }
}