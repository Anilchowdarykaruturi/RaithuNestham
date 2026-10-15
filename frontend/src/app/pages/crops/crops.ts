
import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  CropsService,
  Crop
} from '../../services/crop';

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
    private cropsService: CropsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    console.log('CROPS COMPONENT STARTED');
    this.loadCrops();
  }

  loadCrops(): void {
    console.log('loadCrops() called');

    if (this.loading) {
      console.log('Already loading - stopping duplicate request');
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    console.log('Crops loading = TRUE');
    this.cdr.detectChanges();

    this.cropsService.getCrops().subscribe({

      next: (response: Crop[] | null) => {
        console.log('CROPS API RESPONSE:', response);
        console.log('CROPS COUNT:', response?.length ?? 0);

        this.crops = Array.isArray(response)
          ? response
          : [];

        this.loading = false;

        console.log('CROPS LOADING:', this.loading);
        this.cdr.detectChanges();
      },

      error: (error: any) => {
        console.error('Crops API error:', error);

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load crops information. Please try again.';

        this.loading = false;

        console.log('CROPS LOADING:', this.loading);
        this.cdr.detectChanges();
      },

      complete: () => {
        console.log('Crops request completed');

        this.loading = false;
        this.cdr.detectChanges();
      }

    });
  }
}
