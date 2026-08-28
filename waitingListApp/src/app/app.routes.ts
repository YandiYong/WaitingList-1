import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'check-in',
    loadComponent: () =>
      import('./check-in-page/check-in-page').then((component) => component.CheckInPage),
  },
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./main-page/main-page').then((component) => component.MainPage),
  },
  { path: '**', redirectTo: '' },
];
