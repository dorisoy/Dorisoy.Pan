import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DeletedFileComponent } from './deleted-file.component';

const routes: Routes = [
  {
    path: '',
    component: DeletedFileComponent,
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class DeletedFileRoutingModule { }
