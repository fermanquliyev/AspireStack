import { DOCUMENT } from '@angular/common';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CookieService {
  document: Document = inject(DOCUMENT);
  constructor() {}

  setCookie(name: string, value: string, days: number): void {
    const date = new Date();
    date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
    const expires = 'expires=' + date.toUTCString();
    this.document.cookie = name + '=' + value + ';' + expires + ';path=/';
  }

  getCookie(name: string): string | null {
    const nameEQ = name + '=';
    const ca = this.document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') {
        c = c.substring(1, c.length);
      }
      if (c.indexOf(nameEQ) === 0) {
        return c.substring(nameEQ.length, c.length);
      }
    }
    return null;
  }

  deleteCookie(name: string): void {
    this.document.cookie = name + '=; Max-Age=-99999999;';
  }
}
