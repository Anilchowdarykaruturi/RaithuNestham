import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface Crop {
  id: number;
  name: string;
  teluguName: string;
  season: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class CropsService {

  constructor(private api: ApiService) {}

  getCrops(): Observable<Crop[]> {
    return this.api.get<Crop[]>('Crops');
  }
}