import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepicker, MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MAT_DATE_LOCALE, provideNativeDateAdapter } from '@angular/material/core';

interface Centre {
  id: number;
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  allowedRadiusMetres: number;
}

interface VisitPreview {
  clientNumber: string;
  fullName: string;
  centreName: string;
  visitDate: Date;
}

@Component({
  selector: 'app-main-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
  ],
  providers: [
    {provide: MAT_DATE_LOCALE, useValue:'en-GB'},
    provideNativeDateAdapter(),],
  templateUrl: './main-page.html',
  styleUrl: './main-page.scss',
})
export class MainPage {
  private readonly formBuilder = inject(FormBuilder);

  readonly centres: Centre[] = [
    {
      id: 1,
      name: 'Mthatha Centre',
      address: 'Mthatha, Eastern Cape',
      latitude: -31.5889,
      longitude: 28.7844,
      allowedRadiusMetres: 150,
    },
    {
      id: 2,
      name: 'East London Centre',
      address: 'East London, Eastern Cape',
      latitude: -33.0153,
      longitude: 27.9116,
      allowedRadiusMetres: 150,
    },
    {
      id: 3,
      name: 'Queenstown Centre',
      address: 'Komani, Eastern Cape',
      latitude: -31.8976,
      longitude: 26.8753,
      allowedRadiusMetres: 150,
    },
  ];

  readonly minimumVisitDate = new Date().toISOString().slice(0, 10);
  readonly submittedVisit = signal<VisitPreview | null>(null);

  readonly visitForm = this.formBuilder.group({
    clientNumber: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.pattern(/^[A-Za-z0-9-]{4,20}$/),
    ]),
    fullName: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(100),
    ]),
    phoneNumber: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.pattern(/^(?:\+27|0)[6-8][0-9]{8}$/),
    ]),
    centreId: this.formBuilder.control<number | null>(null, Validators.required),
    visitDate: this.formBuilder.control<Date | null>(null, Validators.required),
  });

  get selectedCentre(): Centre | null {
    const centreId = this.visitForm.controls.centreId.value;
    return this.centres.find((centre) => centre.id === centreId) ?? null;
  }

  submit(): void {
    this.submittedVisit.set(null);

    if (this.visitForm.invalid) {
      this.visitForm.markAllAsTouched();
      return;
    }

    const formValue = this.visitForm.getRawValue();
    const centre = this.centres.find((item) => item.id === formValue.centreId);

    if(!centre || !formValue.visitDate){return}

    if (!centre) {
      this.visitForm.controls.centreId.setErrors({ required: true });
      return;
    }

    this.submittedVisit.set({
      clientNumber: formValue.clientNumber,
      fullName: formValue.fullName,
      centreName: centre.name,
      visitDate: formValue.visitDate,
    });
  }

}
