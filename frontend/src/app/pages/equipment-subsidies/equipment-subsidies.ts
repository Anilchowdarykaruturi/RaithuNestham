import { Component, OnInit, inject } from '@angular/core';
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

  subsidies: EquipmentSubsidy[] = [];

  loading = false;

  errorMessage = '';

  ngOnInit(): void {

    

    this.loadSubsidies();
  }

  loadSubsidies(): void {

    // Prevent duplicate API requests
    if (this.loading) {
      return;
    }

   

    this.loading = true;

    this.errorMessage = '';

    this.equipmentSubsidyService
      .getEquipmentSubsidies()
      .subscribe({

        next: (data: EquipmentSubsidy[]) => {

                 

          this.subsidies = data;

          this.loading = false;

          
      
        },

        error: (error: any) => {

          console.error(
            'Equipment subsidies error:',
            error
          );

          this.errorMessage =
            error?.userMessage ||
            error?.error?.message ||
            'Unable to load equipment subsidies.';

          this.loading = false;
        }

      });
  }
}