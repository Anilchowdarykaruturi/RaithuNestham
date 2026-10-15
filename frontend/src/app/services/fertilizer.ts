
import { Injectable } from '@angular/core';
import { Observable, tap, finalize } from 'rxjs';
import { ApiService } from './api';

export interface Fertilizer {
  id: number;
  name: string;
  type: string;
  recommendedFor: string;
  usageInstructions: string;
  isOrganic: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class FertilizerService {

  constructor(private api: ApiService) {}

  getFertilizers(): Observable<Fertilizer[]> {
    console.log('FERTILIZER SERVICE STARTED');

    return this.api.get<Fertilizer[]>('Fertilizers').pipe(
      tap({
        next: (response) => {
          console.log('FERTILIZERS RESPONSE:', response);
        },
        error: (error) => {
          console.error('FERTILIZERS REQUEST FAILED:', error);
        }
      }),
      finalize(() => {
        console.log('FERTILIZERS REQUEST FINISHED');
      })
    );
  }
}
