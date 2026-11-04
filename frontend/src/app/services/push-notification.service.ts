
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Capacitor } from '@capacitor/core';

import {
  PushNotifications,
  Token,
  PushNotificationSchema,
  ActionPerformed
} from '@capacitor/push-notifications';

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PushNotificationService {
  private http = inject(HttpClient);

  private initialized = false;
  private fcmToken: string | null = null;
  private lastRegistrationKey: string | null = null;
  private registrationInProgress = false;

  private readonly apiUrl =
    `${environment.apiUrl}/DeviceRegistrations/register`;

  async initialize(): Promise<void> {
    if (!Capacitor.isNativePlatform()) {
      console.log(
        'Push notifications require the native Android app.'
      );
      return;
    }

    if (this.initialized) {
      return;
    }

    this.initialized = true;

    try {
      await PushNotifications.addListener(
        'registration',
        (token: Token) => {
          console.log('Push notification registration succeeded.');

          // Keep the token in memory. Never print it in logs.
          this.fcmToken = token.value;

          // Register immediately if the farmer is already logged in.
          this.registerPendingToken();
        }
      );

      await PushNotifications.addListener(
        'registrationError',
        (error) => {
          console.error(
            'Push notification registration failed:',
            error
          );
        }
      );

      await PushNotifications.addListener(
        'pushNotificationReceived',
        (notification: PushNotificationSchema) => {
          console.log(
            'Push notification received:',
            notification.title
          );
        }
      );

      await PushNotifications.addListener(
        'pushNotificationActionPerformed',
        (action: ActionPerformed) => {
          console.log(
            'Notification opened:',
            action.notification.title
          );
        }
      );

      let permission =
        await PushNotifications.checkPermissions();

      if (permission.receive === 'prompt') {
        permission =
          await PushNotifications.requestPermissions();
      }

      if (permission.receive !== 'granted') {
        console.log(
          'The farmer has not granted notification permission.'
        );
        return;
      }

      await PushNotifications.register();

    } catch (error) {
      this.initialized = false;

      console.error(
        'Unable to initialize push notifications:',
        error
      );
    }
  }

  registerPendingToken(): void {
    const jwtToken = localStorage.getItem('raithu_token');

    if (!this.fcmToken || !jwtToken) {
      return;
    }

    const registrationKey =
      `${jwtToken}:${this.fcmToken}`;

    if (
      this.lastRegistrationKey === registrationKey ||
      this.registrationInProgress
    ) {
      return;
    }

    this.registrationInProgress = true;

    this.http.post(
      this.apiUrl,
      {
        fcmToken: this.fcmToken
      }
    ).subscribe({
      next: () => {
        this.lastRegistrationKey = registrationKey;
        this.registrationInProgress = false;

        console.log(
          'Android device registered with RaithuNestham.'
        );
      },
      error: (error) => {
        this.registrationInProgress = false;

        console.error(
          'Unable to register Android device with backend.',
          error.status
        );
      }
    });
  }
}
