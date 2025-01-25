import { Injectable } from '@angular/core';
import { CookieService } from './Cookie.service';
import { AppConstants } from '../app.constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private cookieService: CookieService) {}

  getAuthToken() {
    return this.cookieService.getCookie(AppConstants.AUTH_TOKEN_COOKIE_NAME);
  }

  setAuthToken(token: string, days: number) {
    this.cookieService.setCookie(
      AppConstants.AUTH_TOKEN_COOKIE_NAME,
      token,
      days
    );
  }

  deleteAuthToken() {
    this.cookieService.deleteCookie(AppConstants.AUTH_TOKEN_COOKIE_NAME);
  }
}
