import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedFileComponent } from './shared-file.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: SharedFileComponent,
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class SharedFileRoutingModule { }
