import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedFileComponent } from './shared-file.component';
import { SharedFileRoutingModule } from './shared-file-routing.module';
import { SharedModule } from '@shared/shared.module';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';



@NgModule({
  declarations: [SharedFileComponent],
  imports: [
    CommonModule,
    SharedFileRoutingModule,
    SharedModule,
    MatTooltipModule,
    MatMenuModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule
  ]
})
export class SharedFileModule { }
