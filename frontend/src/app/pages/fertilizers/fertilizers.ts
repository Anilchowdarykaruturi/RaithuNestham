import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FertilizerService,
  Fertilizer
} from '../../services/fertilizer';

@Component({
  selector: 'app-fertilizers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fertilizers.html',
  styleUrl: './fertilizers.css'
})
export class Fertilizers implements OnInit {

  fertilizers: Fertilizer[] = [];

  loading = false;

  errorMessage = '';

  constructor(
    private fertilizerService: FertilizerService
  ) {}

  ngOnInit(): void {
    this.loadFertilizers();
  }

  loadFertilizers(): void {

    // Prevent duplicate API requests
    if (this.loading) {
      return;
    }

    this.loading = true;

    this.errorMessage = '';

    this.fertilizerService.getFertilizers().subscribe({

      next: (response: Fertilizer[]) => {

        

        this.fertilizers = response;

        this.loading = false;
      },

      error: (error: any) => {

        console.error(
          'Fertilizers API error:',
          error
        );

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load fertilizers information.';

        this.loading = false;
      }

    });
  }
}