import { NgTemplateOutlet } from '@angular/common';
import { AfterViewInit, Component, computed, inject, input, ViewChild } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import {
  AvatarComponent,
  BadgeComponent,
  BreadcrumbRouterComponent,
  ColorModeService,
  ContainerComponent,
  DropdownComponent,
  DropdownDividerDirective,
  DropdownHeaderDirective,
  DropdownItemDirective,
  DropdownMenuDirective,
  DropdownToggleDirective,
  HeaderComponent,
  HeaderNavComponent,
  HeaderTogglerDirective,
  NavItemComponent,
  NavLinkDirective,
  SidebarToggleDirective,
} from '@coreui/angular';

import { IconDirective } from '@coreui/icons-angular';
import { AuthService } from '../../../services/auth-service.service';
import { CurrentUserService } from '../../../services/current-user.service';
import { LocalizationService } from 'src/app/services/localization/localization.service';
import { LocalizePipe } from 'src/app/modules/shared/pipes/localization-pipe/localize.pipe';

@Component({
  selector: 'app-default-header',
  templateUrl: './default-header.component.html',
  imports: [
    ContainerComponent,
    HeaderTogglerDirective,
    SidebarToggleDirective,
    IconDirective,
    HeaderNavComponent,
    NavItemComponent,
    NavLinkDirective,
    RouterLink,
    RouterLinkActive,
    NgTemplateOutlet,
    BreadcrumbRouterComponent,
    DropdownComponent,
    DropdownToggleDirective,
    AvatarComponent,
    DropdownMenuDirective,
    DropdownHeaderDirective,
    DropdownItemDirective,
    BadgeComponent,
    DropdownDividerDirective,
    LocalizePipe
  ],
})
export class DefaultHeaderComponent extends HeaderComponent implements AfterViewInit {
  readonly #colorModeService = inject(ColorModeService);
  readonly colorMode = this.#colorModeService.colorMode;
  readonly authService = inject<AuthService>(AuthService);
  readonly router = inject(Router);
  public readonly currentUser = inject<CurrentUserService>(CurrentUserService);
  public readonly localization = inject(LocalizationService);

  @ViewChild('breadCrumbRouter') breadCrumbRouter!: BreadcrumbRouterComponent;

  readonly colorModes = [
    { name: 'light', text: this.L('Light'), icon: 'cilSun' },
    { name: 'dark', text: this.L('Dark'), icon: 'cilMoon' },
    { name: 'auto', text: this.L('Auto'), icon: 'cilContrast' },
  ];

  L(key: string, ...args: any[]): string {
    return this.localization.getTranslation(key, args);
  }

  readonly languages = computed(() => {
    return this.localization
      .getSupportedLanguages()
      .map((lang) => ({
        name: lang,
        text: lang.split('-')[1].toUpperCase(),
        icon: lang.split('-')[1].toLowerCase(),
      }));
  });

  readonly currentLanguageFlag = computed(() => {
    return (
      this.languages().find(
        (lang) => lang.name === this.localization.getCurrentLanguage()
      )?.icon ?? 'us'
    );
  });

  readonly icons = computed(() => {
    const currentMode = this.colorMode();
    return (
      this.colorModes.find((mode) => mode.name === currentMode)?.icon ??
      'cilSun'
    );
  });

  constructor() {
    super();
  }
  ngAfterViewInit(): void {
    // this.breadCrumbRouter.breadcrumbs?.subscribe((crumbs) => {
    //   this.breadCrumbRouter.items = crumbs.map((crumb) => ({
    //     ...crumb,
    //     label: this.L(crumb.label),
    //   }));
    // });
  }

  sidebarId = input('sidebar1');

  public setLanguage(lang: string): void {
    this.localization.setCurrentLanguage(lang);
    window.location.reload();
  }

  logout(): void {
    this.authService.deleteAuthToken();
    this.currentUser.clear();
    this.router.navigate(['/login']);
  }
}
