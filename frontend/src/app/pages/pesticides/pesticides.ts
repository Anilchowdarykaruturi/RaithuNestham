
import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

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
    private pesticideService: PesticideService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    console.log('PESTICIDES COMPONENT STARTED');
    this.loadPesticides();
  }

  loadPesticides(): void {
    if (this.loading) {
      return;
    }

    console.log('loadPesticides() called');

    this.loading = true;
    this.errorMessage = '';

    console.log('Pesticides loading = TRUE');

    this.pesticideService.getPesticides().subscribe({

      next: (response: Pesticide[]) => {
        console.log('PESTICIDES API RESPONSE:', response);
        console.log(
          'PESTICIDES COUNT:',
          response?.length ?? 0
        );

        this.pesticides = Array.isArray(response)
          ? response
          : [];

        this.loading = false;

        console.log('PESTICIDES LOADING:', this.loading);

        this.cdr.detectChanges();
      },

      error: (error: any) => {
        console.error('PESTICIDES API ERROR:', error);

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load pesticide information. Please try again later.';

        this.loading = false;

        console.log('PESTICIDES LOADING:', this.loading);

        this.cdr.detectChanges();
      },

      complete: () => {
        this.loading = false;
        console.log('Pesticides request completed');
        this.cdr.detectChanges();
      }

    });
  }
}
