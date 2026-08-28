import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CheckInResult } from '../models/check-in.models';
import { VisitApiService } from '../Services/visit-api.service';

@Component({
  selector: 'app-check-in-page',
  imports: [DatePipe, MatButtonModule, RouterLink],
  templateUrl: './check-in-page.html',
  styleUrl: './check-in-page.scss',
})
export class CheckInPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly visitApi = inject(VisitApiService);
  protected qrToken = '';

  readonly result = signal<CheckInResult | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly isLoading = signal(true);
  readonly isCheckingIn = signal(false);

  ngOnInit(): void {
    this.qrToken = this.route.snapshot.queryParamMap.get('token')?.trim() ?? '';

    if (!this.qrToken) {
      this.errorMessage.set('The QR code does not contain a valid appointment token.');
      this.isLoading.set(false);
      return;
    }

    this.validateScan();
  }

  validateScan(): void {
    this.errorMessage.set(null);
    this.isLoading.set(true);

    this.visitApi.getCheckInResult(this.qrToken).subscribe({
      next: (result) => {
        this.result.set(result);
        this.isLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(this.getApiError(error));
        this.isLoading.set(false);
      },
    });
  }

  checkIn(): void {
    this.errorMessage.set(null);

    if (!navigator.geolocation) {
      this.errorMessage.set('Location is not supported by this browser.');
      return;
    }

    this.isCheckingIn.set(true);

    // The browser asks the visitor for permission before sharing coordinates.
    navigator.geolocation.getCurrentPosition(
      (position) => this.completeCheckIn(position.coords.latitude, position.coords.longitude),
      (error) => {
        this.errorMessage.set(this.getLocationError(error));
        this.isCheckingIn.set(false);
      },
      {
        enableHighAccuracy: true,
        timeout: 15_000,
        maximumAge: 0,
      },
    );
  }

  private completeCheckIn(latitude: number, longitude: number): void {
    this.visitApi.checkIn({ qrToken: this.qrToken, latitude, longitude }).subscribe({
      next: (result) => {
        this.result.set(result);
        this.isCheckingIn.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(this.getApiError(error));
        this.isCheckingIn.set(false);
      },
    });
  }

  private getApiError(error: HttpErrorResponse): string {
    return error.error?.detail ??
      error.error?.title ??
      'Check-in is temporarily unavailable. Please try again.';
  }

  private getLocationError(error: GeolocationPositionError): string {
    switch (error.code) {
      case 1:
        return 'Location permission is required. Allow location access and try again.';
      case 2:
        return 'Your current location could not be found. Check your location settings and try again.';
      case 3:
        return 'Finding your location took too long. Please try again.';
      default:
        return 'Your location could not be checked. Please try again.';
    }
  }
}
