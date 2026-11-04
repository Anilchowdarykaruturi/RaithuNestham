
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {

  username = '';
  password = '';
  loading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {

    // If a saved session exists, skip the login page.
    if (this.authService.isLoggedIn()) {
      this.router.navigateByUrl('/dashboard', {
        replaceUrl: true
      });
    }
  }

  login(): void {

    this.errorMessage = '';
    this.username = this.username.trim();

    if (!this.username) {
      this.errorMessage = 'Please enter your username.';
      return;
    }

    if (!this.password) {
      this.errorMessage = 'Please enter your password.';
      return;
    }

    if (this.loading) {
      return;
    }

    this.loading = true;

    this.authService
      .login(this.username, this.password)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({

        next: (response) => {

          if (!response?.token) {
            this.errorMessage =
              'Login response did not contain an authentication token.';
            return;
          }

          this.router.navigateByUrl('/dashboard', {
            replaceUrl: true
          });
        },

        error: (error) => {

          console.error('Login failed:', error);

          if (error.status === 0) {
            this.errorMessage =
              'Cannot connect to the server. Check your internet connection.';
          } else if (error.status === 401) {
            this.errorMessage =
              'Invalid username or password.';
          } else if (error.status === 400) {
            this.errorMessage =
              error.error?.message || 'Please check your login details.';
          } else if (error.status >= 500) {
            this.errorMessage =
              'The server encountered an error. Please try again later.';
          } else {
            this.errorMessage =
              error.error?.message || 'Login failed. Please try again.';
          }
        }
      });
  }
}
