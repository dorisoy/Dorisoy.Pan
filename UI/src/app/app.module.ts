import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { HttpInterceptorModule } from './http-interceptor.module';
import { AppStoreModule } from './store/app-store.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PendingInterceptorModule } from '@shared/loading-indicator/pending-interceptor.module';
import { EmptyComponent } from './empty/empty.component';

import { MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { MatPaginatorIntlCN } from './core/paginator/paginator.translate'


@NgModule({
  declarations: [
    AppComponent,
    EmptyComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    CoreModule,
    BrowserAnimationsModule,
    HttpClientModule,
    HttpInterceptorModule,
    AppStoreModule,
    PendingInterceptorModule,
    ToastrModule.forRoot(),
    MatPaginatorModule
  ],
  providers: [ 
    { 
      provide: MatPaginatorIntl, 
      useClass: MatPaginatorIntlCN
     }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
