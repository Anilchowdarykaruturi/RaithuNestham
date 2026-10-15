
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private http = inject(HttpClient);

  private baseUrl = environment.apiUrl;

 get<T>(endpoint: string): Observable<T> {
  const url = `${this.baseUrl}/${endpoint}`;

  console.log('API REQUEST URL:', url);

  return this.http.get<T>(url);
}
  post<T>(endpoint: string, data: unknown): Observable<T> {
    return this.http.post<T>(
      `${this.baseUrl}/${endpoint}`,
      data
    );
  }

  put<T>(endpoint: string, data: unknown): Observable<T> {
    return this.http.put<T>(
      `${this.baseUrl}/${endpoint}`,
      data
    );
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(
      `${this.baseUrl}/${endpoint}`
    );
  }
}

