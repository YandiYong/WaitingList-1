import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainPage } from './main-page';

describe('MainPage', () => {
  let component: MainPage;
  let fixture: ComponentFixture<MainPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MainPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should reject an empty visit request', () => {
    component.submit();

    expect(component.visitForm.invalid).toBeTrue();
    expect(component.submittedVisit()).toBeNull();
    expect(component.visitForm.controls.clientNumber.touched).toBeTrue();
  });

  it('should create a local visit preview for valid details', () => {
    component.visitForm.setValue({
      clientNumber: 'CL-1024',
      fullName: 'Nandi Dlamini',
      phoneNumber: '0821234567',
      centreId: 1,
      visitDate: component.minimumVisitDate,
    });

    component.submit();

    expect(component.visitForm.valid).toBeTrue();
    expect(component.submittedVisit()).toEqual({
      clientNumber: 'CL-1024',
      fullName: 'Nandi Dlamini',
      centreName: 'Mthatha Centre',
      visitDate: component.minimumVisitDate,
    });
  });
});
