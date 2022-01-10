import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription, timer } from 'rxjs';
import { debounce } from 'rxjs/operators';
import { PendingInterceptorService } from './pending-interceptor.service';

@Component({
  selector: 'app-loading-indicator',
  templateUrl: './loading-indicator.component.html',
  styleUrls: ['./loading-indicator.component.scss']
})
export class LoadingIndicatorComponent implements OnInit, OnDestroy {

  public isSpinnerVisible: boolean;
  @Input()
  public backgroundColor: string;
  @Input()
  public filteredUrlPatterns: string[] = [];
  @Input()
  public debounceDelay = 100;
  @Input()
  public entryComponent: any = null;
  private subscription: Subscription;

  constructor(private pendingRequestInterceptorService: PendingInterceptorService) {
    this.subscription = this.pendingRequestInterceptorService
      .pendingRequestsStatus
      .pipe(debounce(this.handleDebounce.bind(this)))
      .subscribe(hasPendingRequests => this.isSpinnerVisible = hasPendingRequests);
  }

  ngOnInit(): void {
    if (!(this.filteredUrlPatterns instanceof Array)) {
      throw new TypeError('`filteredUrlPatterns` must be an array.');
    }

    if (!!this.filteredUrlPatterns.length) {
      this.filteredUrlPatterns.forEach(e => {
        this.pendingRequestInterceptorService.filteredUrlPatterns.push(new RegExp(e));
      });
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private handleDebounce(hasPendingRequests: boolean): Observable<number> {
    if (hasPendingRequests) {
      return timer(this.debounceDelay);
    }

    return timer(0);
  }
}
