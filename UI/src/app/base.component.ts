import { Component, OnDestroy } from '@angular/core';
import { SubSink } from 'SubSink';

@Component({
  selector: 'app-base',
  template: ``
})
export class BaseComponent implements OnDestroy {
  sub$: SubSink;
  constructor() {
    this.sub$ = new SubSink();
  }
  ngOnDestroy(): void {
    this.sub$.unsubscribe();
  }
}
