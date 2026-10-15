
import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  GovernmentSchemeService,
  GovernmentScheme
} from '../../services/government-scheme';

@Component({
  selector: 'app-government-schemes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './government-schemes.html',
  styleUrl: './government-schemes.css'
})
export class GovernmentSchemes implements OnInit {

  schemes: GovernmentScheme[] = [];
  loading = false;
  errorMessage = '';

  constructor(
    private governmentSchemeService: GovernmentSchemeService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadGovernmentSchemes();
  }

  loadGovernmentSchemes(): void {
    if (this.loading) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.governmentSchemeService
      .getGovernmentSchemes()
      .subscribe({

        next: (response: GovernmentScheme[]) => {
          this.schemes = response ?? [];
          this.loading = false;

          this.cdr.detectChanges();
        },

        error: (error: any) => {
          console.error('Government schemes API error:', error);

          this.errorMessage =
            error?.userMessage ||
            error?.error?.message ||
            'Unable to load government schemes.';

          this.loading = false;

          this.cdr.detectChanges();
        },

        complete: () => {
          this.loading = false;
          this.cdr.detectChanges();
        }

      });
  }
}
