
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../environments/environment';
import { PushNotificationService } from './push-notification.service';

export interface AuthResponse {
  token: string;
  userId: number;
  farmerId: number;
  username: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);

  private pushNotificationService =
    inject(PushNotificationService);

  private apiUrl = `${environment.apiUrl}/Auth`;

  // Login
  login(
    username: string,
    password: string
  ): Observable<AuthResponse> {

    return this.http.post<AuthResponse>(
      `${this.apiUrl}/login`,
      { username, password }
    ).pipe(
      tap(response => this.saveSession(response))
    );
  }

  // Register
  register(
    username: string,
    password: string,
    fullName: string,
    phoneNumber: string,
    village: string,
    mandal: string,
    district: string
  ): Observable<AuthResponse> {

    return this.http.post<AuthResponse>(
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
      tap(response => this.saveSession(response))
    );
  }

  // Save authentication session
  private saveSession(response: AuthResponse): void {

    if (!response?.token) {
      console.error('[AUTH] Login response has no token.');
      return;
    }

    try {
      localStorage.setItem('raithu_token', response.token);

      if (response.farmerId > 0) {
        localStorage.setItem(
          'raithu_farmer_id',
          String(response.farmerId)
        );
      } else {
        localStorage.removeItem('raithu_farmer_id');
      }

      console.log('[AUTH] Token saved:',
        !!localStorage.getItem('raithu_token'));

      console.log('[AUTH] Farmer ID saved:',
        this.getFarmerId() > 0);

      this.pushNotificationService.registerPendingToken();

    } catch (error) {
      console.error('[AUTH] Could not save session:', error);
    }
  }

  // Get JWT token
  getToken(): string | null {
    return localStorage.getItem('raithu_token');
  }

  // Get farmer ID
  getFarmerId(): number {
    return Number(
      localStorage.getItem('raithu_farmer_id') || 0
    );
  }

  // Check login status
  isLoggedIn(): boolean {

    const token = this.getToken();

    console.log('[AUTH CHECK] Token exists:', !!token);

    console.log(
      '[AUTH CHECK] Farmer ID exists:',
      this.getFarmerId() > 0
    );

    return !!token;
  }

  // Restore an existing saved session
  restoreSession(): boolean {

    const token = this.getToken();

    console.log('[AUTH RESTORE] Token exists:', !!token);

    if (!token) {
      console.log('[AUTH RESTORE] No saved session found.');
      return false;
    }

    this.pushNotificationService.registerPendingToken();

    console.log('[AUTH RESTORE] Saved session detected.');

    return true;
  }

  // Logout
  logout(): void {

    localStorage.removeItem('raithu_token');
    localStorage.removeItem('raithu_farmer_id');

    console.log('[AUTH] Session cleared.');
  }
}
