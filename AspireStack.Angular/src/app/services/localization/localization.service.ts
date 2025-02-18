import { computed, inject, Injectable, signal, WritableSignal } from '@angular/core';
import { CookieService } from '../Cookie.service';
import { ApiService } from '../api-services/api-service-proxies';

@Injectable({
  providedIn: 'root'
})
export class LocalizationService {

  private currentLanguage: string = 'en-US';
  private cookieService: CookieService = inject(CookieService);
  private apiService: ApiService = inject(ApiService);
  private currentTranslation: WritableSignal<{
    [key: string]: string
  }> = signal({});
  private supportedLanguages = signal(["en-US", "fr-FR", "tr-TR", "ru-RU", "az-AZ"]);
constructor() { }


  public getCurrentLanguage(): string {
    if (this.currentLanguage === 'en-US') {
      this.currentLanguage = this.cookieService.getCookie('language') ?? 'en-US';
    }
    return this.currentLanguage;
  }

  public setCurrentLanguage(language: string): void {
    this.currentLanguage = language;
    this.cookieService.setCookie('language', language, 365);
  }

  public loadTranslations(){
    this.apiService.getCurrentLocalization().subscribe((response) => {
      this.currentTranslation.set(response.resources!);
      this.supportedLanguages.set(response.supportedCultures!);
    });
  }

  public getSupportedLanguages = computed(() => {
    return [...this.supportedLanguages()];
  });

  public getTranslation(key: string, ...args: any[]): string {
    if (args.length > 0) {
      return this.formatString(this.currentTranslation()[key] ?? key, args);
    }
    return this.currentTranslation()[key] ?? key;
  }

  private formatString(str: string, ...args: any[]): string {
    return str.replace(/{(\d+)}/g, (match, number) => {
      return typeof args[number] != 'undefined' ? args[number] : match;
    });
  }
}
