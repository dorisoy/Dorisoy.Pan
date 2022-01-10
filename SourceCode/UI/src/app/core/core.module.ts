import { ErrorHandler, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { CommonDialogService } from './common-dialog/common-dialog.service';
import { MatDialogModule } from '@angular/material/dialog';
import { SharedModule } from '@shared/shared.module';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoadingIndicatorModule } from '@shared/loading-indicator/loading-indicator.module';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { CommonErrorHandlerService } from './error-handler/common-error-handler.service';
import { HttpClient } from '@angular/common/http';
import { MatTreeModule } from '@angular/material/tree';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RightSidebarComponent } from './right-sidebar/right-sidebar.component';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminSidebrComponent } from './admin-layout/admin-sidebr/admin-sidebr.component';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  declarations: [
    LayoutComponent,
    HeaderComponent,
    FooterComponent,
    SidebarComponent,
    RightSidebarComponent,
    AdminLayoutComponent,
    AdminSidebrComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    RouterModule,
    SharedModule,
    MatTooltipModule,
    LoadingIndicatorModule,
    PerfectScrollbarModule,
    MatTreeModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    MatButtonModule,
    MatIconModule
  ],
  exports: [
    LayoutComponent
  ],
  providers: [
    CommonDialogService,
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    },
    {
      provide: ErrorHandler, useClass: CommonErrorHandlerService,
      deps: [HttpClient]
    }
  ]
})
export class CoreModule { }
