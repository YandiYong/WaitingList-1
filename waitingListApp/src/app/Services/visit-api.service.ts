import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Centre,
  CreateVisitRequest,
  Visit,
} from '../models/visit.models';

@Injectable({ providedIn: 'root' })
export class VisitApiService {
  private readonly http = inject(HttpClient);

  // Keep the backend address in one place while the app is in development.
  private readonly apiUrl = 'http://localhost:5239/api';

  getCentres(): Observable<Centre[]> {
    return this.http.get<Centre[]>(`${this.apiUrl}/centres`);
  }

  getVisits(): Observable<Visit[]> {
    return this.http.get<Visit[]>(`${this.apiUrl}/visits`);
  }

  createVisit(request: CreateVisitRequest): Observable<Visit> {
    return this.http.post<Visit>(`${this.apiUrl}/visits`, request);
  }
}
