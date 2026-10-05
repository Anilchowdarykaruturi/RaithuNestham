import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  username: string = '';
  password: string = '';

  loading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {

    this.errorMessage = '';

    // Remove extra spaces
    this.username = this.username.trim();

    // Validate username
    if (!this.username) {
      this.errorMessage = 'Please enter your username.';
      return;
    }

    // Validate password
    if (!this.password) {
      this.errorMessage = 'Please enter your password.';
      return;
    }

    // Prevent duplicate login requests
    if (this.loading) {
      return;
    }

    this.loading = true;

    

    this.authService
      .login(this.username, this.password)
      .subscribe({

        next: (response) => {

          

          this.loading = false;

          this.router.navigate(['/dashboard']);
        },

        error: (error) => {

          console.error('Login failed:', error);

          this.loading = false;

          this.errorMessage =
            error?.userMessage ||
            error?.error?.message ||
            'Login failed. Please check your username and password.';
        }

      });
  }
}