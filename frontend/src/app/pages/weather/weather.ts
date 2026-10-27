
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  WeatherService,
  WeatherLog,
  WeatherAdviceResponse,
  WeatherForecastDay
} from '../../services/weather';

@Component({
  selector: 'app-weather',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './weather.html',
  styleUrl: './weather.css'
})
export class Weather implements OnInit {

  // Current weather
  village = '';
  weather: WeatherLog[] = [];
  loading = false;
  errorMessage = '';

  // Seven-day forecast
  forecast: WeatherForecastDay[] = [];
  forecastLoading = false;
  forecastError = '';

  // Crop advice
  cropName = 'paddy';
  growthStage = 'growing';

  latitude = 16.977152;
  longitude = 81.80249;

  advice: WeatherAdviceResponse | null = null;
  adviceLoading = false;
  adviceError = '';

  constructor(private weatherService: WeatherService) {}

  ngOnInit(): void {
    this.loadWeather();
    this.loadForecast();
  }

  // Load current weather
  loadWeather(): void {
    if (this.loading) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.weatherService.getWeather(this.village).subscribe({
      next: (response) => {
        this.weather = response;
        this.loading = false;
      },
      error: (error) => {
        console.error('Weather API error:', error);

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load weather information.';

        this.loading = false;
      }
    });
  }

  // Load seven-day forecast
  loadForecast(): void {
    if (this.forecastLoading) {
      return;
    }

    this.forecastLoading = true;
    this.forecastError = '';

    this.weatherService.getForecast(
      this.latitude,
      this.longitude
    ).subscribe({
      next: (response) => {
        const daily = response.daily;

        this.forecast = daily.time.map((date, index) => ({
          date,
          maxTemperature: daily.temperature_2m_max[index],
          minTemperature: daily.temperature_2m_min[index],
          rainProbability:
            daily.precipitation_probability_max[index],
          rainfall: daily.precipitation_sum[index],
          windSpeed: daily.wind_speed_10m_max[index]
        }));

        this.forecastLoading = false;
      },
      error: (error) => {
        console.error('Weather forecast API error:', error);

        this.forecastError =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load the seven-day forecast. Please try again.';

        this.forecastLoading = false;
      }
    });
  }

  // Load Telugu farming advice for the selected crop
  loadAdvice(): void {
    if (this.adviceLoading) {
      return;
    }

    if (!this.cropName.trim() || !this.growthStage.trim()) {
      this.adviceError =
        'Please select a crop and growth stage.';
      return;
    }

    this.adviceLoading = true;
    this.adviceError = '';
    this.advice = null;

    this.weatherService.getWeatherAdvice(
      this.cropName,
      this.growthStage,
      this.latitude,
      this.longitude
    ).subscribe({
      next: (response) => {
        this.advice = response;
        this.adviceLoading = false;
      },
      error: (error) => {
        console.error('Farming advice API error:', error);

        this.adviceError =
          error?.error?.message ||
          error?.userMessage ||
          'Unable to load farming advice. Please try again.';

        this.adviceLoading = false;
      }
    });
  }
}
