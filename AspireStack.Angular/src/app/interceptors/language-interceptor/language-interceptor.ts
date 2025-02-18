import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LocalizationService } from '../../services/localization/localization.service';

@Injectable()
export class LanguageInterceptorService implements HttpInterceptor {
  intercept(req: HttpRequest<any>, handler: HttpHandler): Observable<HttpEvent<any>> {
  const language = inject(LocalizationService).getCurrentLanguage();
  const newReq = req.clone({
    headers: req.headers.append('Accept-Language', language)
  });
  return handler.handle(newReq);
  }
}
