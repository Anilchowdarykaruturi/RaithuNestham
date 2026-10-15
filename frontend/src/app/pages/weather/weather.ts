import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WeatherService, WeatherLog } from '../../services/weather';

@Component({
  selector: 'app-weather',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './weather.html',
  styleUrl: './weather.css'
})
export class Weather implements OnInit {

  village = '';
  weather: WeatherLog[] = [];
  loading = false;
  errorMessage = '';

  constructor(private weatherService: WeatherService) {}

  ngOnInit(): void {
    console.log('WEATHER COMPONENT STARTED');
    this.loadWeather();
  }

  loadWeather(): void {

    console.log('loadWeather() called');

    if (this.loading) {
      console.log('Already loading - stopping duplicate request');
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    console.log('Loading = TRUE');

    this.weatherService.getWeather(this.village).subscribe({
      next: (response) => {
        console.log('Weather response:', response);

        this.weather = response;
        this.loading = false;

        console.log('Loading = FALSE');
      },

      error: (error) => {
        console.error('Weather API error:', error);

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load weather information.';

        this.loading = false;

        console.log('Loading = FALSE because of ERROR');
      },

      complete: () => {
        console.log('Weather request completed');
        this.loading = false;
      }
    });
  }
}