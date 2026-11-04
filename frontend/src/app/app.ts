
import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PushNotificationService } from './services/push-notification.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private pushNotificationService =
    inject(PushNotificationService);

  ngOnInit(): void {
    void this.pushNotificationService.initialize();
  }
}
