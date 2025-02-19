import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./role-management.component').then(m => m.RoleManagementComponent),
    data: {
      title: `Role Management`
    }
  }
];

