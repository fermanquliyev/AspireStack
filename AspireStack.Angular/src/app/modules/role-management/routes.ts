import { Routes } from '@angular/router';
import '@angular/localize/init';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./role-management.component').then(m => m.RoleManagementComponent),
    data: {
      title: $localize`Role Management`
    }
  }
];

