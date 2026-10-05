
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AIChatRequest {
  farmerId: number;
  question: string;
}

export interface AIChatResponse {
  question: string;
  answer: string;
}

@Injectable({
  providedIn: 'root'
})
export class AiService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/AI`;

  chat(request: AIChatRequest): Observable<AIChatResponse> {
    return this.http.post<AIChatResponse>(
      `${this.apiUrl}/chat`,
      request
    );
  }
}

