import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  PesticideService,
  Pesticide
} from '../../services/pesticide';

@Component({
  selector: 'app-pesticides',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pesticides.html',
  styleUrl: './pesticides.css'
})
export class Pesticides implements OnInit {

  pesticides: Pesticide[] = [];

  loading = false;

  errorMessage = '';

  constructor(
    private pesticideService: PesticideService
  ) {}

  ngOnInit(): void {
    this.loadPesticides();
  }

  loadPesticides(): void {

    // Prevent duplicate API requests
    if (this.loading) {
      return;
    }

    this.loading = true;

    this.errorMessage = '';

    this.pesticideService
      .getPesticides()
      .subscribe({

        next: (response: Pesticide[]) => {

         

          this.pesticides = response;

          this.loading = false;
        },

        error: (error: any) => {

          console.error(
            'Pesticides API error:',
            error
          );

          this.errorMessage =
            error?.userMessage ||
            error?.error?.message ||
            'Unable to load pesticide information.';

          this.loading = false;
        }

      });
  }
}