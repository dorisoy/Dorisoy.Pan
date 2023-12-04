import {Injectable} from '@angular/core';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {BehaviorSubject} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class BreakpointsService {
    public isMobile$ = new BehaviorSubject(false);
    public isTablet$ = new BehaviorSubject(false);

    constructor(private breakpointObserver: BreakpointObserver) {
        this.breakpointObserver.observe(Breakpoints.Handset).subscribe(result => {
            this.isMobile$.next(result.matches);
        });

        this.breakpointObserver.observe(Breakpoints.Tablet).subscribe(result => {
            this.isTablet$.next(result.matches);
        });
    }

    public observe(value: string) {
        return this.breakpointObserver.observe(value);
    }
}
