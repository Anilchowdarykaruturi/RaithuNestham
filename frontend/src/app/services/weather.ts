import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface WeatherLog {
  id: number;
  village: string;
  temperature: number;
  humidity: number;
  rainfall: number;
  weatherCondition: string;
  recordedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class WeatherService {

  constructor(private api: ApiService) {}

  getWeather(village?: string): Observable<WeatherLog[]> {

    let endpoint = 'Weather';

    if (village && village.trim() !== '') {
      endpoint += `?village=${encodeURIComponent(village)}`;
    }

    return this.api.get<WeatherLog[]>(endpoint);
  }
}
