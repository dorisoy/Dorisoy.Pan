import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StarredFileComponent } from './starred-file.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: StarredFileComponent,
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ], exports: [
    RouterModule
  ]
})
export class StarredFileRoutingModule { }
