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
          icon: 'nav-icon-bullet'
        },
        {
          name: $localize`Role Managment`,
          url: '/role-management',
          icon: 'nav-icon-bullet'
        },
      ]
  },
  {
    title: true,
    name: 'Theme'
  },
  {
    name: 'Colors',
    url: '/theme/colors',
    iconComponent: { name: 'cil-drop' }
  },
  {
    name: 'Typography',
    url: '/theme/typography',
    linkProps: { fragment: 'headings' },
    iconComponent: { name: 'cil-pencil' }
  }
  // {
  //   name: 'Docs',
  //   url: 'https://coreui.io/angular/docs/',
  //   iconComponent: { name: 'cil-description' },
  //   attributes: { target: '_blank' }
  // }
];
