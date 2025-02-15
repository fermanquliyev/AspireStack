import {
    HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import Swal from 'sweetalert2'

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor() {}

  intercept(
    req: HttpRequest<any>,
    handler: HttpHandler
  ): Observable<HttpEvent<any>> {
    return handler.handle(req).pipe(
      catchError((errorResponse: HttpErrorResponse) => {
        // Handle the error here.
        (errorResponse.error as Blob).text().then((text) => {
            const errorData = JSON.parse(text);
            let content = '';
            if(errorData.data != null){
                if(errorData.data instanceof String){
                    content = errorData.data + '<br>';
                } else if(errorData.data instanceof Array){
                    for(let i = 0; i < errorData.data.length; i++){
                        content += errorData.data[i] + '<br>';
                    }
                }
            }
            content += `Status code: ${errorData.statusCode}<br>`;
            Swal.fire({
              title: errorData.message,
              icon: 'error',
              confirmButtonText: 'Close',
              html: content
            });
        });
        
        // Rethrow the error to pass it down the chain.
        throw errorResponse;
      })
    );
  }
}
