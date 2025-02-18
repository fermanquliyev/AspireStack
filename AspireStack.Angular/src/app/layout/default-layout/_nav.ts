import { INavData } from '@coreui/angular';
import "@angular/localize/init";

export const navItems: INavData[] = [
  {
    name: $localize`Dashboard`,
    url: '/dashboard',
    iconComponent: { name: 'cil-speedometer' },
    badge: {
      color: 'info',
      text: 'NEW'
    }
  },
  {
      name: $localize`Identity`,
      iconComponent: { name: 'cil-user' },
      children: [
        {
          name: $localize`User Managment`,
          url: '/user-management',
          icon: 'nav-icon-bullet',
          attributes: { permission: 'UserManagement' }
        },
        {
          name: $localize`Role Managment`,
          url: '/role-management',
          icon: 'nav-icon-bullet',
          attributes: { permission: 'RoleManagement' }
        },
      ]
  }
];
