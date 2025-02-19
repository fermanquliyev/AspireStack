/// <reference types="@angular/localize" />

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { CurrentUserService } from './app/services/current-user.service';
import { AppConstants } from './app/app.constants';
import { LocalizationService } from './app/services/localization/localization.service';

const getAllPermissions = () => {
  const xhr = new XMLHttpRequest();
  xhr.open('GET', '/api/Role/GetAllPermissions', false); // false makes the request synchronous
  xhr.setRequestHeader('Authorization', 'Bearer ' + localStorage.getItem(AppConstants.AUTH_TOKEN_COOKIE_NAME));
  xhr.onreadystatechange = () => {
    if (xhr.readyState === 4) {
      if (xhr.status === 200) {
        const data = JSON.parse(xhr.responseText);
        CurrentUserService.setAllPermissions(data);
      } else {
        console.error('Error getAllPermissions', xhr.statusText);
      }
    }
  };
  xhr.send();
}

const gettingTranslations = () => {
  const xhr = new XMLHttpRequest();
  xhr.open('GET', '/api/Localization/GetCurrentLocalization', false); // false makes the request synchronous
  xhr.setRequestHeader('Accept-Language', LocalizationService.getCurrentLanguage());
  xhr.onreadystatechange = () => {
    if (xhr.readyState === 4) {
      if (xhr.status === 200) {
        const data = JSON.parse(xhr.responseText);
        LocalizationService.setTranslations(data.resources, data.supportedCultures);
      } else {
        console.error('Error Getting translations', xhr.statusText);
      }
    }
  };
  xhr.send();
}

const environmentInitializer = () => {
  CurrentUserService.loadUserFromToken();
  getAllPermissions();
  gettingTranslations();
}

environmentInitializer();

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
