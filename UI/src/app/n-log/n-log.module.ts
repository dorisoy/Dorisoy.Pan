import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NLogListComponent } from './n-log-list/n-log-list.component';
import { NLogRoutingModule } from './n-log-routing.module';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { SharedModule } from '@shared/shared.module';
import { MatSelectModule } from '@angular/material/select';
import { NLogDetailComponent } from './n-log-detail/n-log-detail.component';
import { LogDetailResolverService } from './log-detail-resolver';



@NgModule({
  declarations: [NLogListComponent, NLogDetailComponent],
  imports: [
    CommonModule,
    SharedModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatPaginatorModule,
    MatInputModule,
    MatSelectModule,
    NLogRoutingModule
  ],
  providers: [LogDetailResolverService]
})
export class NLogModule { }
