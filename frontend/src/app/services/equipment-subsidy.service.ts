
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface EquipmentSubsidy {
  id: number;
  equipmentName: string;
  category: string;
  schemeName: string;
  eligibleFarmers: string;
  subsidyDetails: string;
  maximumSubsidy: string;
  applicationProcess: string;
  officialWebsite: string;
  state: string;
  lastVerified: string;
}

@Injectable({
  providedIn: 'root'
})
export class EquipmentSubsidyService {

  private http = inject(HttpClient);

  private apiUrl =
    `${environment.apiUrl}/EquipmentSubsidies`;

  getEquipmentSubsidies(): Observable<EquipmentSubsidy[]> {

    const token = localStorage.getItem('raithu_token');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<EquipmentSubsidy[]>(
      this.apiUrl,
      { headers }
    );
  }

  getEquipmentSubsidy(
    id: number
  ): Observable<EquipmentSubsidy> {

    const token = localStorage.getItem('raithu_token');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<EquipmentSubsidy>(
      `${this.apiUrl}/${id}`,
      { headers }
    );
  }
}

