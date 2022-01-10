import { Injectable } from '@angular/core';
import {
    Resolve,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';
import { Folder } from '@core/domain-classes/folder';
import { CommonService } from '@core/services/common.service';
import { Observable } from 'rxjs';

@Injectable({
    providedIn:'root'
})
export class RootResolver implements Resolve<Folder> {
    constructor(private commonService: CommonService) { }
    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<Folder> {
        return this.commonService.getRootFolder() as Observable<Folder>;
    }
}
