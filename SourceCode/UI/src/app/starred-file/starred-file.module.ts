import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StarredFileComponent } from './starred-file.component';
import { StarredFileRoutingModule } from './starred-file-routing.module';
import { SharedModule } from '@shared/shared.module';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';



@NgModule({
  declarations: [StarredFileComponent],
  imports: [
    CommonModule,
    StarredFileRoutingModule,
    SharedModule,
    MatTooltipModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ]
})
export class StarredFileModule { }
