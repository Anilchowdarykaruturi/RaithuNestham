
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginResponse {
  token: string;
}

interface RegisterResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Auth`;

  login(
    username: string,
    password: string
  ): Observable<LoginResponse> {

    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      {
        username,
        password
      }
    ).pipe(
      tap(response => {

        if (response?.token) {
          localStorage.setItem(
            'raithu_token',
            response.token
          );
        }

      })
    );
  }

  register(
    username: string,
    password: string,
    fullName: string,
    phoneNumber: string,
    village: string,
    mandal: string,
    district: string
  ): Observable<RegisterResponse> {

    return this.http.post<RegisterResponse>(
      `${this.apiUrl}/register`,
      {
        username,
        password,
        fullName,
        phoneNumber,
        village,
        mandal,
        district
      }
    ).pipe(
      tap(response => {

        if (response?.token) {
          localStorage.setItem(
            'raithu_token',
            response.token
          );
        }

      })
    );
  }

  logout(): void {
    localStorage.removeItem('raithu_token');
  }

  getToken(): string | null {
    return localStorage.getItem('raithu_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}

