import { Injectable, NgModule } from '@angular/core';
import { Observable } from 'rxjs';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { environment } from '@environments/environment';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class HttpRequestInterceptor implements HttpInterceptor {
  /**
   *
   */
  constructor(private router: Router, private toastrService: ToastrService) {
  }
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    var token = localStorage.getItem('bearerToken');
    const baseUrl = environment.apiUrl;
    if (req.url.lastIndexOf('i18n') > -1) {
      return next.handle(req);
    }
    const url = req.url.lastIndexOf('api') > -1 ? req.url : 'api/' + req.url;
    if (token) {
      const newReq = req.clone({
        headers: req.headers.set('Authorization', 'Bearer ' + token),
        url: `${baseUrl}${url}`,
      });
      return next.handle(newReq).pipe(
        tap(
          () => { },
          (err: any) => {
            if (err instanceof HttpErrorResponse) {
              if (err.status === 401) {
                this.router.navigate(['login']);
              } else if (err.error && err.error instanceof Blob) {
                // Nothing to do here
              }
              else if (err.error && Array.isArray(err.error)) {
                this.toastrService.error(err.error.join(" </br> "), "", {
                  enableHtml: true
                });
              } else {
                this.toastrService.error(err.error.errors["id"][0], "", {
                  enableHtml: true
                });
              }
            }
          }
        )
      );
    } else {
      const newReq = req.clone({
        url: `${baseUrl}${url}`,
      });
      return next.handle(newReq).pipe(
        tap(
          () => { },
          (err: any) => {
            if (err instanceof HttpErrorResponse) {
              if (err.status === 401) {
                this.router.navigate(['login']);
              }
              if (err.status === 403) {
                this.router.navigate(['login']);
              }
              if (err.status === 409) {
                this.toastrService.error(err.error.messages[0])
              }
            }
          }
        )
      );
    }
  }
}

@NgModule({
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpRequestInterceptor,
      multi: true,
    },
  ],
})
export class HttpInterceptorModule { }
