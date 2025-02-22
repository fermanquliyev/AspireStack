import { AfterViewInit, Component, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { NgScrollbar } from 'ngx-scrollbar';

import { IconDirective } from '@coreui/icons-angular';
import {
  ContainerComponent,
  ShadowOnScrollDirective,
  SidebarBrandComponent,
  SidebarComponent,
  SidebarFooterComponent,
  SidebarHeaderComponent,
  SidebarNavComponent,
  SidebarToggleDirective,
  SidebarTogglerDirective
} from '@coreui/angular';

import { DefaultFooterComponent, DefaultHeaderComponent } from './';
import { navItems } from './_nav';
import { AppBaseComponent } from '../../modules/shared/components/base-component/AppBaseComponent';

function isOverflown(element: HTMLElement) {
  return (
    element.scrollHeight > element.clientHeight ||
    element.scrollWidth > element.clientWidth
  );
}

@Component({
    selector: 'app-dashboard',
    templateUrl: './default-layout.component.html',
    styleUrls: ['./default-layout.component.scss'],
    imports: [
        SidebarComponent,
        SidebarHeaderComponent,
        SidebarBrandComponent,
        RouterLink,
        NgScrollbar,
        SidebarNavComponent,
        SidebarFooterComponent,
        SidebarToggleDirective,
        SidebarTogglerDirective,
        DefaultHeaderComponent,
        ShadowOnScrollDirective,
        ContainerComponent,
        RouterOutlet,
        DefaultFooterComponent
    ]
})
export class DefaultLayoutComponent extends AppBaseComponent implements AfterViewInit {
  
  public menuItems = [...navItems];

  onScrollbarUpdate($event: any) {
    // if ($event.verticalUsed) {
    // console.log('verticalUsed', $event.verticalUsed);
    // }
  }

  ngAfterViewInit(): void {
    this.menuItems = navItems.filter((item) => {
      if(!!item.attributes && item.attributes['permission']){
        return this.hasPermission(item.attributes['permission']);
      }
      return true;
    });

    this.menuItems.forEach((item) => {
      if (item.children) {
        item.children = item.children.filter((child) => {
          if(!!child.attributes && child.attributes['permission']){
            return this.hasPermission(child.attributes['permission']);
          }
          return true;
        });
      }
    });
  }
}
