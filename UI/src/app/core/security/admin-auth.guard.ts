import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
  CanActivateChild,
  CanLoad,
  Route
} from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { SecurityService } from './security.service';

@Injectable({ providedIn: 'root' })
export class AdminAuthGuard implements CanActivate, CanActivateChild, CanLoad {
  constructor(
    private securityService: SecurityService,
    private router: Router,
  ) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (!this.securityService.isAdminLogin()) {
      this.router.navigate(['login'], {
        queryParams: { returnUrl: state.url }
      });
      return false;
    }
    return true;
  }

  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (!this.securityService.isAdminLogin()) {
      this.router.navigate(['login'], {
        queryParams: { returnUrl: state.url }
      });
      return false;
    }
    return true;
  }
  canLoad(route: Route): boolean {
    if (this.securityService.isAdminLogin()) {
      return true;
    } else {
      this.router.navigate(['login']);
      return false;
    }
  }
}
