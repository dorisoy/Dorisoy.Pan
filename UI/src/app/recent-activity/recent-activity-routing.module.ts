import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecentActivityComponent } from './recent-activity.component';

const routes: Routes = [
  {
    path:'',
    component: RecentActivityComponent
  }
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RecentActivityRoutingModule { }
