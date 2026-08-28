import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { QRCodeComponent } from 'angularx-qrcode';
import {
  Centre,
  CreateVisitRequest,
  Visit,
} from '../models/visit.models';
import { VisitApiService } from '../Services/visit-api.service';
import {
  formatLocalDate,
  strictDateProviders,
} from '../shared/strict-date.adapter';

@Component({
  selector: 'app-main-page',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    QRCodeComponent,
  ],
  providers: strictDateProviders,
  templateUrl: './main-page.html',
  styleUrl: './main-page.scss',
})
export class MainPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly visitApi = inject(VisitApiService);

  readonly centres = signal<Centre[]>([]);
  readonly visits = signal<Visit[]>([]);
  readonly submittedVisit = signal<Visit | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly isLoading = signal(false);
  readonly isSubmitting = signal(false);

  // This uses the browser's local date, not UTC.
  readonly minimumVisitDate = this.startOfLocalToday();

  readonly visitForm = this.formBuilder.group({
    accNumber: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.pattern(/^[A-Za-z0-9-]{4,20}$/),
    ]),
    fullName: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(100),
    ]),
    cellNumber: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.pattern(/^(?:\+27|0)[6-8][0-9]{8}$/),
    ]),
    centreId: this.formBuilder.control<number | null>(null, Validators.required),
    visitDate: this.formBuilder.control<Date | null>(null, Validators.required),
  });

  get selectedCentre(): Centre | null {
    const centreId = this.visitForm.controls.centreId.value;
    return this.centres().find((centre) => centre.centreId === centreId) ?? null;
  }

  ngOnInit(): void {
    this.loadCentres();
    this.loadVisits();
  }

  submit(): void {
    this.submittedVisit.set(null);
    this.errorMessage.set(null);

    if (this.visitForm.invalid) {
      this.visitForm.markAllAsTouched();
      return;
    }

    const value = this.visitForm.getRawValue();
    if (value.centreId === null || value.visitDate === null) {
      return;
    }

    const request: CreateVisitRequest = {
      accNumber: value.accNumber.trim(),
      fullName: value.fullName.trim(),
      cellNumber: value.cellNumber.trim(),
      centreId: value.centreId,
      // The API also enforces this exact format.
      visitDate: formatLocalDate(value.visitDate),
    };

    this.isSubmitting.set(true);

    this.visitApi.createVisit(request).subscribe({
      next: (visit) => {
        this.submittedVisit.set(visit);
        this.isSubmitting.set(false);
        this.loadVisits();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(this.getApiError(error));
        this.isSubmitting.set(false);
      },
    });
  }

  getQrValue(visit: Visit): string {
    // The QR contains only a secure token URL, never the patient's details.
    return `${window.location.origin}/check-in?token=${encodeURIComponent(visit.qrToken)}`;
  }

  private loadCentres(): void {
    this.visitApi.getCentres().subscribe({
      next: (centres) => this.centres.set(centres),
      error: (error: HttpErrorResponse) =>
        this.errorMessage.set(this.getApiError(error)),
    });
  }

  private loadVisits(): void {
    this.isLoading.set(true);

    this.visitApi.getVisits().subscribe({
      next: (visits) => {
        this.visits.set(visits);
        this.isLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(this.getApiError(error));
        this.isLoading.set(false);
      },
    });
  }

  private getApiError(error: HttpErrorResponse): string {
    return error.error?.detail ??
      error.error?.title ??
      'The backend could not be reached. Make sure the API is running.';
  }

  private startOfLocalToday(): Date {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), now.getDate());
  }
}
