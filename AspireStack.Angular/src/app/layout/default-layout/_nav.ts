import { INavData } from '@coreui/angular';

export const navItems: INavData[] = [
  {
    name: 'Dashboard',
    url: '/dashboard',
    iconComponent: { name: 'cil-speedometer' },
    badge: {
      color: 'info',
      text: 'NEW'
    }
  },
  {
      name: 'Identity',
      iconComponent: { name: 'cil-user' },
      children: [
        {
          name: 'User Managment',
          url: '/user-management',
          icon: 'nav-icon-bullet',
          attributes: { permission: 'UserManagement' }
        },
        {
          name: 'Role Managment',
          url: '/role-management',
          icon: 'nav-icon-bullet',
          attributes: { permission: 'RoleManagement' }
        },
      ]
  }
];
