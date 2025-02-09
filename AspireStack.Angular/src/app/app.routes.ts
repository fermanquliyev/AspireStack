import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot, Routes } from '@angular/router';
import { DefaultLayoutComponent } from './layout';
import { CurrentUserService } from './services/current-user.service';
import { inject } from '@angular/core';

const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const currentUserService = inject(CurrentUserService);
  if (currentUserService.getIsAuthenticated()) {
    return true;
  } else {
    console.log('Not authenticated, redirecting to login');
    console.log({...currentUserService})
    return inject(Router).createUrlTree(['/login']);
  }
};

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: '',
    component: DefaultLayoutComponent,
    data: {
      title: 'Home'
    },
    children: [
      {
        path: 'dashboard',
        loadChildren: () => import('./modules/dashboard/routes').then((m) => m.routes),
        //canActivate: [authGuard]
      },
      {
        path: 'user-management',
        loadChildren: () => import('./modules/user-management/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'role-management',
        loadChildren: () => import('./modules/role-management/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'theme',
        loadChildren: () => import('./modules/theme/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'base',
        loadChildren: () => import('./modules/base/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'buttons',
        loadChildren: () => import('./modules/buttons/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'forms',
        loadChildren: () => import('./modules/forms/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'icons',
        loadChildren: () => import('./modules/icons/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'notifications',
        loadChildren: () => import('./modules/notifications/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'widgets',
        loadChildren: () => import('./modules/widgets/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'charts',
        loadChildren: () => import('./modules/charts/routes').then((m) => m.routes),
        canActivate: [authGuard]
      },
      {
        path: 'pages',
        loadChildren: () => import('./modules/pages/routes').then((m) => m.routes),
        canActivate: [authGuard]
      }
    ]
  },
  {
    path: '404',
    loadComponent: () => import('./modules/pages/page404/page404.component').then(m => m.Page404Component),
    data: {
      title: 'Page 404',
      showInMenu: false
    }
  },
  {
    path: '500',
    loadComponent: () => import('./modules/pages/page500/page500.component').then(m => m.Page500Component),
    data: {
      title: 'Page 500',
      showInMenu: false
    }
  },
  {
    path: 'login',
    loadComponent: () => import('./modules/pages/login/login.component').then(m => m.LoginComponent),
    data: {
      title: 'Login Page',
      showInMenu: false
    }
  },
  {
    path: 'register',
    loadComponent: () => import('./modules/pages/register/register.component').then(m => m.RegisterComponent),
    data: {
      title: 'Register Page',
      showInMenu: false
    }
  },
  { path: '**', redirectTo: 'dashboard' }
];

