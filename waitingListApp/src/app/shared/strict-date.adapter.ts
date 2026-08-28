import { Injectable } from '@angular/core';
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
  MatDateFormats,
  NativeDateAdapter,
} from '@angular/material/core';

export const strictDateFormat = 'dd/MM/yyyy';

export const strictDateFormats: MatDateFormats = {
  parse: { dateInput: strictDateFormat },
  display: {
    dateInput: strictDateFormat,
    monthYearLabel: 'MMM yyyy',
    dateA11yLabel: 'dd MMMM yyyy',
    monthYearA11yLabel: 'MMMM yyyy',
  },
};

/** Accepts manually entered dates only when they use dd/MM/yyyy. */
@Injectable()
export class StrictDateAdapter extends NativeDateAdapter {
  override parse(value: unknown): Date | null {
    if (value instanceof Date) {
      return this.isValid(value) ? value : null;
    }

    if (typeof value !== 'string' || value.length === 0) {
      return null;
    }

    const match = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(value);
    if (!match) {
      return null;
    }

    const day = Number(match[1]);
    const month = Number(match[2]) - 1;
    const year = Number(match[3]);
    const date = new Date(year, month, day);

    return date.getFullYear() === year &&
      date.getMonth() === month &&
      date.getDate() === day
      ? date
      : null;
  }

  override format(date: Date, displayFormat: any): string {
    if (displayFormat === strictDateFormat) {
      return formatLocalDate(date);
    }

    return super.format(date, displayFormat);
  }
}

/** Formats a Date without converting it to UTC. */
export function formatLocalDate(date: Date): string {
  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  return `${day}/${month}/${date.getFullYear()}`;
}

export const strictDateProviders = [
  { provide: DateAdapter, useClass: StrictDateAdapter },
  { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
  { provide: MAT_DATE_FORMATS, useValue: strictDateFormats },
];
