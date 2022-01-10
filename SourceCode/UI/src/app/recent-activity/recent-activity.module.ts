import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecentActivityRoutingModule } from './recent-activity-routing.module';
import { RecentActivityComponent } from './recent-activity.component';
import { PipesModule } from '@shared/pipes/pipes.module';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { SharedModule } from '@shared/shared.module';


@NgModule({
  declarations: [
    RecentActivityComponent
  ],
  imports: [
    CommonModule,
    RecentActivityRoutingModule,
    SharedModule,
    PipesModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatProgressSpinnerModule
  ]
})
export class RecentActivityModule { }
