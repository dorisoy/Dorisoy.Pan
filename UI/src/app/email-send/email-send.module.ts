import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmailSendComponent } from './email-send.component';
import { EmailSendRoutingModule } from './email-send-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { NgxEditorModule } from 'ngx-editor';
import { SharedModule } from '@shared/shared.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';



@NgModule({
  declarations: [EmailSendComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatSelectModule,
    EmailSendRoutingModule,
    NgxEditorModule,
    SharedModule,
    MatProgressSpinnerModule
  ]
})
export class EmailSendModule { }
