
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface WeatherLog {
  id: number | null;
  village: string;
  temperature: number;
  humidity: number;
  rainfall: number;
  weatherCondition: string;
  recordedAt: string;
  source?: string;
}

export interface WeatherForecast {
  latitude: number;
  longitude: number;
  timezone: string;
  daily: {
    time: string[];
    temperature_2m_max: number[];
    temperature_2m_min: number[];
    precipitation_probability_max: number[];
    precipitation_sum: number[];
    wind_speed_10m_max: number[];
  };
}

export interface WeatherForecastDay {
  date: string;
  maxTemperature: number;
  minTemperature: number;
  rainProbability: number;
  rainfall: number;
  windSpeed: number;
}

export interface WeatherAdvice {
  cropName: string;
  growthStage: string;
  rainProbabilityPercent: number;
  expectedRainfallMm: number;
  maximumWindSpeedKmh: number;
  harvestAdvice: string;
  fertilizerAdvice: string;
  pesticideAdvice: string;
}

export interface WeatherAdviceResponse {
  forecastDate: string;
  advice: WeatherAdvice;
}

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  constructor(private api: ApiService) {}

  getWeather(village?: string): Observable<WeatherLog[]> {
    let endpoint = 'Weather';

    if (village && village.trim() !== '') {
      endpoint += `?village=${encodeURIComponent(village.trim())}`;
    }

    return this.api.get<WeatherLog[]>(endpoint);
  }

  getForecast(
    latitude = 16.977152,
    longitude = 81.80249
  ): Observable<WeatherForecast> {
    const params = new URLSearchParams({
      latitude: latitude.toString(),
      longitude: longitude.toString()
    });

    return this.api.get<WeatherForecast>(
      `Weather/forecast?${params.toString()}`
    );
  }

  getWeatherAdvice(
    cropName: string,
    growthStage: string,
    latitude = 16.977152,
    longitude = 81.80249
  ): Observable<WeatherAdviceResponse> {
    const params = new URLSearchParams({
      cropName,
      growthStage,
      latitude: latitude.toString(),
      longitude: longitude.toString()
    });

    return this.api.get<WeatherAdviceResponse>(
      `Weather/advice?${params.toString()}`
    );
  }
}
