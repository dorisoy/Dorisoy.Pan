import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeletedFileComponent } from './deleted-file.component';
import { DeletedFileRoutingModule } from './deleted-file-routing.module';
import { SharedModule } from '@shared/shared.module';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';



@NgModule({
  declarations: [DeletedFileComponent],
  imports: [
    CommonModule,
    DeletedFileRoutingModule,
    SharedModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule
  ]
})
export class DeletedFileModule { }
