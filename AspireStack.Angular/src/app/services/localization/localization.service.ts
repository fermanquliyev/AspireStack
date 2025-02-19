import { computed, inject, Injectable, signal, WritableSignal } from '@angular/core';
import { ApiService } from '../api-services/api-service-proxies';
import { tap } from 'rxjs';
import { AppConstants } from 'src/app/app.constants';

@Injectable({
  providedIn: 'root'
})
export class LocalizationService {

  private currentLanguage: string = AppConstants.DEFAULT_LANGUAGE;
  private apiService: ApiService = inject(ApiService);
  private static currentTranslation: {
    [key: string]: string
  } = {};
  private static supportedLanguages = AppConstants.SUPPORTED_LANGUAGES;
constructor() { }


  public getCurrentLanguage(): string {
    if (this.currentLanguage === AppConstants.DEFAULT_LANGUAGE) {
      this.currentLanguage = localStorage.getItem(AppConstants.LANGUAGE_COOKIE_NAME) ?? AppConstants.DEFAULT_LANGUAGE;
    }
    return this.currentLanguage;
  }

  public static getCurrentLanguage(): string {
    return  localStorage.getItem(AppConstants.LANGUAGE_COOKIE_NAME) ?? AppConstants.DEFAULT_LANGUAGE;
  }

  public setCurrentLanguage(language: string): void {
    this.currentLanguage = language;
    localStorage.setItem(AppConstants.LANGUAGE_COOKIE_NAME, language);
  }

  public loadTranslations(){
    return this.apiService.getCurrentLocalization().pipe(tap((response)=>{
      LocalizationService.currentTranslation = response.resources!;
      LocalizationService.supportedLanguages = response.supportedCultures!;
    }));
  }

  public static setTranslations(resources: any, supportedLanguages: any){
    LocalizationService.currentTranslation = resources;
    LocalizationService.supportedLanguages = supportedLanguages
  }

  public getSupportedLanguages() {
    return [...LocalizationService.supportedLanguages];
  };

  public getTranslation(key: string, ...args: any[]): string {
    if (args.length > 0) {
      return this.formatString(LocalizationService.currentTranslation[key] ?? key + ' NF!', args);
    }
    return LocalizationService.currentTranslation[key] ?? key + ' NF!';
  }

  private formatString(str: string, ...args: any[]): string {
    return str.replace(/{(\d+)}/g, (match, number) => {
      return typeof args[number] != 'undefined' ? args[number] : match;
    });
  }
}
