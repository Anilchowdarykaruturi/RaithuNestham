import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface GovernmentScheme {
  id: number;
  name: string;
  description: string;
  eligibility: string;
  benefits: string;
  applicationProcess: string;
  officialWebsite: string;
}

@Injectable({
  providedIn: 'root'
})
export class GovernmentSchemeService {

  constructor(private api: ApiService) {}

  getGovernmentSchemes(): Observable<GovernmentScheme[]> {
    return this.api.get<GovernmentScheme[]>(
      'GovernmentSchemes'
    );
  }
}