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

private cdr = inject(ChangeDetectorRef);

private sanitizer = inject(DomSanitizer);

farmerId = 1;

question = '';

loading = false;

error = '';

messages: ChatMessage[] = [
{
sender: 'ai',
text: 'Namaste! How can I help you with your farming needs today?',
html: 'Namaste! How can I help you with your farming needs today?'
}
];

askQuestion(): void {


const questionText = this.question.trim();

/*
 * Prevent duplicate requests.
 */
if (this.loading) {
  return;
}

/*
 * Validate the question.
 */
if (!questionText) {
  this.error = 'Please enter a farming question.';
  return;
}

this.error = '';

/*
 * Add user's question to the conversation.
 */
this.messages.push({
  sender: 'user',
  text: questionText
});

/*
 * Clear the input.
 */
this.question = '';

/*
 * Show loading state.
 */
this.loading = true;

/*
 * Send question to backend.
 */
this.aiService.chat({
  farmerId: this.farmerId,
  question: questionText
}).subscribe({

  /*
   * Successful response.
   */
  next: (response: AIChatResponse) => {

    const formattedHtml =
      this.formatAiResponse(response.answer);

    this.messages.push({
      sender: 'ai',
      text: response.answer,
      html: formattedHtml
    });

    this.loading = false;

    this.cdr.detectChanges();
  },

  /*
   * API error.
   */
  error: (error: any) => {

    this.error =
      error?.userMessage ||
      error?.error?.message ||
      'Unable to get a response from AI.';

    this.loading = false;

    this.cdr.detectChanges();
  }

});


}

/*

* Press Enter to send.
* Shift + Enter creates a new line.
  */
  onEnter(event: KeyboardEvent): void {


if (



  event.key === 'Enter' &&
  !event.shiftKey
) {

  event.preventDefault();

  this.askQuestion();
}


}

/*

* Convert AI Markdown-style text into safe HTML.
  */
  private formatAiResponse(response: string): SafeHtml {


let html = response;



/*
 * Escape HTML first so AI output cannot inject HTML/JavaScript.
 */
html = html
  .replace(/&/g, '&amp;')
  .replace(/</g, '&lt;')
  .replace(/>/g, '&gt;')
  .replace(/"/g, '&quot;')
  .replace(/'/g, '&#039;');

/*
 * Telugu heading.
 *
 * తెలుగు:
 *
 * becomes:
 *
 * <div class="language-heading telugu-heading">
 *   తెలుగు
 * </div>
 */
html = html.replace(
  /(^|\n)\s*తెలుగు\s*:/gi,
  '$1<div class="language-heading telugu-heading">తెలుగు</div>'
);

/*
 * English heading.
 *
 * English:
 *
 * becomes:
 *
 * <div class="language-heading english-heading">
 *   English
 * </div>
 */
html = html.replace(
  /(^|\n)\s*English\s*:/gi,
  '$1<div class="language-heading english-heading">English</div>'
);

/*
 * Convert Markdown bold.
 *
 * **text**
 *
 * becomes:
 *
 * <strong>text</strong>
 */
html = html.replace(
  /\*\*(.*?)\*\*/g,
  '<strong>$1</strong>'
);

/*
 * Convert Markdown bullet lines.
 *
 * * Urea
 *
 * becomes:
 *
 * • Urea
 */
html = html.replace(
  /^\s*\*\s+/gm,
  '• '
);

/*
 * Convert horizontal rules.
 *
 * ---
 *
 * becomes:
 *
 * <hr>
 */
html = html.replace(
  /^\s*---+\s*$/gm,
  '<hr>'
);

/*
 * Preserve line breaks and blank lines.
 */
html = html.replace(
  /\r?\n/g,
  '<br>'
);

return this.sanitizer.bypassSecurityTrustHtml(html);


}
}
