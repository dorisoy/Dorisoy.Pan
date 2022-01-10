import { Injectable } from '@angular/core';
import {
    Resolve,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';
import { NLog } from '@core/domain-classes/n-log';
import { User } from '@core/domain-classes/user';
import { Observable } from 'rxjs';
import { NLogService } from './n-log.service';

@Injectable()
export class LogDetailResolverService implements Resolve<NLog> {
    constructor(private nLogService: NLogService) { }
    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<NLog> {
        const id = route.paramMap.get('id');
        return this.nLogService.getLogDetails(id) as Observable<NLog>;
    }
}
