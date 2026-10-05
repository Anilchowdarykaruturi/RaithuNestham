import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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
    return this.api.get<Fertilizer[]>('Fertilizers');
  }
}