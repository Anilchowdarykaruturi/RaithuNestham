
import {
  Component,
  OnInit,
  ChangeDetectorRef,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  EquipmentSubsidy,
  EquipmentSubsidyService
} from '../../services/equipment-subsidy.service';

@Component({
  selector: 'app-equipment-subsidies',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './equipment-subsidies.html',
  styleUrl: './equipment-subsidies.css'
})
export class EquipmentSubsidiesComponent implements OnInit {

  private equipmentSubsidyService = inject(
    EquipmentSubsidyService
  );

  private cdr = inject(ChangeDetectorRef);

  subsidies: EquipmentSubsidy[] = [];

  loading = false;

  errorMessage = '';

  ngOnInit(): void {
    console.log('EQUIPMENT SUBSIDIES COMPONENT STARTED');
    this.loadSubsidies();
  }

  loadSubsidies(): void {

    if (this.loading) {
      console.log('Already loading equipment subsidies');
      return;
    }

    console.log('loadSubsidies() called');

    this.loading = true;
    this.errorMessage = '';

    console.log('Equipment subsidies loading = TRUE');

    this.cdr.detectChanges();

    this.equipmentSubsidyService
      .getEquipmentSubsidies()
      .subscribe({

        next: (data: EquipmentSubsidy[] | null) => {

          console.log(
            'EQUIPMENT SUBSIDIES API RESPONSE:',
            data
          );

          console.log(
            'EQUIPMENT SUBSIDIES COUNT:',
            data?.length ?? 0
          );

          this.subsidies = Array.isArray(data)
            ? data
            : [];

          this.loading = false;

          console.log(
            'EQUIPMENT SUBSIDIES LOADING:',
            this.loading
          );

          this.cdr.detectChanges();
        },

        error: (error: any) => {

          console.error(
            'EQUIPMENT SUBSIDIES API ERROR:',
            error
          );

          this.errorMessage =
            error?.userMessage ||
            error?.error?.message ||
            'Unable to load equipment subsidies. Please try again.';

          this.loading = false;

          console.log(
            'EQUIPMENT SUBSIDIES LOADING:',
            this.loading
          );

          this.cdr.detectChanges();
        },

        complete: () => {

          console.log(
            'Equipment subsidies request completed'
          );

          this.loading = false;

          this.cdr.detectChanges();
        }

      });
  }
}
