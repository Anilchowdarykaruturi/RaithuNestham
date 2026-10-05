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

  constructor(
    private weatherService: WeatherService
  ) {}

  ngOnInit(): void {
    this.loadWeather();
  }

  loadWeather(): void {

    // Prevent duplicate API requests
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

        console.error(
          'Weather API error:',
          error
        );

        this.errorMessage =
          error?.userMessage ||
          error?.error?.message ||
          'Unable to load weather information.';

        this.loading = false;
      }

    });
  }
}