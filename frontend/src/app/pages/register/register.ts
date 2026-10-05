import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  username = '';
  password = '';
  confirmPassword = '';

  fullName = '';
  phoneNumber = '';
  village = '';
  mandal = '';
  district = '';

  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register(): void {

    this.errorMessage = '';
    this.successMessage = '';

    // Remove unnecessary spaces
    this.username = this.username.trim();
    this.fullName = this.fullName.trim();
    this.phoneNumber = this.phoneNumber.trim();
    this.village = this.village.trim();
    this.mandal = this.mandal.trim();
    this.district = this.district.trim();

    // Required fields
    if (
      !this.username ||
      !this.password ||
      !this.confirmPassword ||
      !this.fullName ||
      !this.phoneNumber ||
      !this.village ||
      !this.mandal ||
      !this.district
    ) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    // Password confirmation
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    // Prevent duplicate requests
    if (this.loading) {
      return;
    }

    this.loading = true;

    
    this.authService
      .register(
        this.username,
        this.password,
        this.fullName,
        this.phoneNumber,
        this.village,
        this.mandal,
        this.district
      )
      .subscribe({

        next: (response) => {

          

          this.loading = false;

          this.successMessage =
            'Registration successful! Redirecting to login...';

          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1500);
        },

        error: (error) => {

          console.error(
            'Registration failed:',
            error
          );

          this.loading = false;

          this.errorMessage =
            error?.userMessage ||
            error?.error?.message ||
            'Registration failed. Please try again.';
        }

      });
  }
}