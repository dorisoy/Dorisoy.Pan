import { Injectable } from '@angular/core';
import {
    Resolve,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';
import { User } from '@core/domain-classes/user';
import { Observable } from 'rxjs';
import { UserService } from './user.service';

@Injectable()
export class UserDetailResolverService implements Resolve<User> {
    constructor(private userService: UserService) { }
    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<User> {
        const id = route.paramMap.get('id');
        return this.userService.getUser(id) as Observable<User>;
    }
}
