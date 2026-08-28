import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { CheckInResult } from '../models/check-in.models';
import { VisitApiService } from '../Services/visit-api.service';
import { CheckInPage } from './check-in-page';

describe('CheckInPage', () => {
  let fixture: ComponentFixture<CheckInPage>;
  let visitApi: jasmine.SpyObj<VisitApiService>;

  const queueResult: CheckInResult = {
    scanSuccessful: true,
    message: 'QR code scanned and check-in completed successfully.',
    centreName: 'Mthatha Centre',
    visitDate: '28/08/2026',
    appointmentStatus: 'CheckedIn',
    queueNumber: 'A037',
    queueStatus: 'WAITING',
    peopleAhead: 4,
    joinedAt: '2026-08-28T11:00:00+02:00',
  };

  beforeEach(async () => {
    visitApi = jasmine.createSpyObj<VisitApiService>('VisitApiService', [
      'getCheckInResult',
      'checkIn',
    ]);
    visitApi.getCheckInResult.and.returnValue(of(queueResult));

    await TestBed.configureTestingModule({
      imports: [CheckInPage],
      providers: [
        { provide: VisitApiService, useValue: visitApi },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { queryParamMap: convertToParamMap({ token: 'valid-token' }) } },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CheckInPage);
    fixture.detectChanges();
  });

  it('should display the assigned queue information', () => {
    const text = fixture.nativeElement.textContent;

    expect(text).toContain('Check-in successful');
    expect(text).toContain('A037');
    expect(text).toContain('WAITING');
    expect(text).toContain('4');
  });
});
