import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Centre, Visit } from '../models/visit.models';
import { VisitApiService } from '../Services/visit-api.service';
import { MainPage } from './main-page';

describe('MainPage', () => {
  let component: MainPage;
  let fixture: ComponentFixture<MainPage>;
  let visitApi: jasmine.SpyObj<VisitApiService>;

  const centre: Centre = {
    centreId: 1,
    externalCentreId: 'MTH-001',
    centreName: 'Mthatha Centre',
    address: 'Mthatha, Eastern Cape',
    latitude: -31.5889,
    longitude: 28.7844,
    allowedRadiusMetres: 150,
  };

  const savedVisit: Visit = {
    visitId: 'dd0e8459-85a6-4807-b441-50947f74b96a',
    accNumber: 'ACC-1024',
    fullName: 'Nandi Dlamini',
    cellNumber: '0821234567',
    centreId: 1,
    centreName: 'Mthatha Centre',
    visitDate: '28/08/2026',
    status: 'Scheduled',
    createdAt: '2026-08-28T09:00:00+02:00',
  };

  beforeEach(async () => {
    visitApi = jasmine.createSpyObj<VisitApiService>('VisitApiService', [
      'getCentres',
      'getVisits',
      'createVisit',
    ]);
    visitApi.getCentres.and.returnValue(of([centre]));
    visitApi.getVisits.and.returnValue(of([]));
    visitApi.createVisit.and.returnValue(of(savedVisit));

    await TestBed.configureTestingModule({
      imports: [MainPage],
      providers: [{ provide: VisitApiService, useValue: visitApi }],
    }).compileComponents();

    fixture = TestBed.createComponent(MainPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load centres when created', () => {
    expect(component.centres()).toEqual([centre]);
  });

  it('should reject an empty visit request', () => {
    component.submit();

    expect(component.visitForm.invalid).toBeTrue();
    expect(visitApi.createVisit).not.toHaveBeenCalled();
  });

  it('should send the visit date as dd/MM/yyyy', () => {
    component.visitForm.setValue({
      accNumber: 'ACC-1024',
      fullName: 'Nandi Dlamini',
      cellNumber: '0821234567',
      centreId: 1,
      visitDate: new Date(2026, 7, 28),
    });

    component.submit();

    expect(visitApi.createVisit).toHaveBeenCalledWith({
      accNumber: 'ACC-1024',
      fullName: 'Nandi Dlamini',
      cellNumber: '0821234567',
      centreId: 1,
      visitDate: '28/08/2026',
    });
    expect(component.submittedVisit()).toEqual(savedVisit);
  });
});
