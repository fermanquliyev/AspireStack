import { Injectable } from '@angular/core';
import { Constants } from '../../constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor() {}

  getAuthToken() {
    return localStorage.getItem(Constants.AUTH_TOKEN_COOKIE_NAME);
  }

  static getAuthToken() {
    return localStorage.getItem(Constants.AUTH_TOKEN_COOKIE_NAME);
  }

  setAuthToken(token: string) {
    localStorage.setItem(
      Constants.AUTH_TOKEN_COOKIE_NAME,
      token
    );
  }

  deleteAuthToken() {
    localStorage.removeItem(Constants.AUTH_TOKEN_COOKIE_NAME);
  }
}
