import { Injectable } from '@angular/core';
import { AppConstants } from '../app.constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor() {}

  getAuthToken() {
    return localStorage.getItem(AppConstants.AUTH_TOKEN_COOKIE_NAME);
  }

  static getAuthToken() {
    return localStorage.getItem(AppConstants.AUTH_TOKEN_COOKIE_NAME);
  }

  setAuthToken(token: string) {
    localStorage.setItem(
      AppConstants.AUTH_TOKEN_COOKIE_NAME,
      token
    );
  }

  deleteAuthToken() {
    localStorage.removeItem(AppConstants.AUTH_TOKEN_COOKIE_NAME);
  }
}
