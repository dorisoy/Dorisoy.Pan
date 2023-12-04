import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart, Router } from '@angular/router';
import { Observable, ReplaySubject, throwError } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';

@Injectable()
export class PendingInterceptorService implements HttpInterceptor {
  private _pendingRequests = 0;

  get pendingRequests(): number {
    return this._pendingRequests;
  }

  private _pendingRequestsStatus: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  get pendingRequestsStatus(): Observable<boolean> {
    return this._pendingRequestsStatus.asObservable();
  }

  private _filteredUrlPatterns: RegExp[] = [];

  get filteredUrlPatterns(): RegExp[] {
    return this._filteredUrlPatterns;
  }

  constructor(router: Router) {
    router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this._pendingRequestsStatus.next(true);
      }

      if ((event instanceof NavigationError || event instanceof NavigationEnd || event instanceof NavigationCancel)) {
        this._pendingRequestsStatus.next(false);
      }
    });
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const shouldBypass = this.shouldBypass(req.url);

    if (!shouldBypass) {
      this._pendingRequests++;

      if (1 === this._pendingRequests) {
        this._pendingRequestsStatus.next(true);
      }
    }

    return next.handle(req).pipe(
      map(event => {
        return event;
      }),
      catchError(error => {
        return throwError(error);
      }),
      finalize(() => {
        if (!shouldBypass) {
          this._pendingRequests--;

          if (0 === this._pendingRequests) {
            this._pendingRequestsStatus.next(false);
          }
        }
      })
    );
  }

  private shouldBypass(url: string): boolean {
    return this._filteredUrlPatterns.some(e => {
      return e.test(url);
    });
  }
}

export function PendingInterceptorServiceFactory(router: Router) {
  return new PendingInterceptorService(router);
}

export let PendingInterceptorServiceFactoryProvider = {
  provide: PendingInterceptorService,
  useFactory: PendingInterceptorServiceFactory,
  deps: [Router]
};
