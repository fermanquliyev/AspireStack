import { Routes } from '@angular/router';
import '@angular/localize/init';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./user-management.component').then(m => m.UserManagementComponent),
    data: {
      title: $localize`User Management`
    }
  }
];

