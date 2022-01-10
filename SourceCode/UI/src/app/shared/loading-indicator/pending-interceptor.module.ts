import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PendingInterceptorService, PendingInterceptorServiceFactoryProvider } from './pending-interceptor.service';

const PendingInterceptorServiceExistingProvider = {
  provide: HTTP_INTERCEPTORS,
  useExisting: PendingInterceptorService,
  multi: true
};

@NgModule({
  imports: [
    CommonModule,
    RouterModule
  ],
  declarations: [],
  providers: [
    PendingInterceptorServiceExistingProvider,
    PendingInterceptorServiceFactoryProvider
  ],
})
export class PendingInterceptorModule {
}
