
import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

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
    private fertilizerService: FertilizerService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    console.log('FERTILIZERS COMPONENT STARTED');
    this.loadFertilizers();
  }

  loadFertilizers(): void {
    if (this.loading) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    console.log('CALLING FERTILIZERS API');

    this.fertilizerService.getFertilizers().subscribe({
      next: (response: Fertilizer[]) => {
        console.log('FERTILIZERS RECEIVED:', response.length);

        this.fertilizers = response ?? [];
        this.loading = false;

        this.cdr.detectChanges();

        console.log('LOADING AFTER RESPONSE:', this.loading);
        console.log('FERTILIZER COUNT:', this.fertilizers.length);
      },

      error: (error: any) => {
        console.error('FERTILIZERS API ERROR:', error);

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load fertilizers information.';

        this.loading = false;

        this.cdr.detectChanges();

        console.log('LOADING AFTER ERROR:', this.loading);
      },

      complete: () => {
        this.loading = false;
        this.cdr.detectChanges();

        console.log('FERTILIZERS REQUEST COMPLETED');
      }
    });
  }
}
