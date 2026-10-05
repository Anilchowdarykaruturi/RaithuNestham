import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CropsService, Crop } from '../../services/crop';

@Component({
  selector: 'app-crops',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './crops.html',
  styleUrl: './crops.css'
})
export class Crops implements OnInit {

  crops: Crop[] = [];
  loading = false;
  errorMessage = '';

  constructor(
    private cropsService: CropsService
  ) {}

  ngOnInit(): void {
    this.loadCrops();
  }

  loadCrops(): void {

    // Prevent duplicate API requests
    if (this.loading) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.cropsService.getCrops().subscribe({

      next: (response: Crop[]) => {

        

        this.crops = response;

        this.loading = false;
      },

      error: (error: any) => {

        console.error(
          'Crops API error:',
          error
        );

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load crops information.';

        this.loading = false;
      }

    });
  }
}