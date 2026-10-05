import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface Pesticide {
  id: number;
  name: string;
  targetPest: string;
  recommendedCrop: string;
  usageInstructions: string;
}

@Injectable({
  providedIn: 'root'
})
export class PesticideService {

  constructor(private api: ApiService) {}

  getPesticides(): Observable<Pesticide[]> {
    return this.api.get<Pesticide[]>('Pesticides');
  }
}