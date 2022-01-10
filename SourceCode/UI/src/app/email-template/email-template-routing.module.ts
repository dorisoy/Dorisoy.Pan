import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmailTemplateListComponent } from './email-template-list/email-template-list.component';
import { EmailTemplateManageComponent } from './email-template-manage/email-template-manage.component';
import { EmailTemplateResolver } from './email-template.resolver';

const routes: Routes = [
  {
    path: '',
    component: EmailTemplateListComponent,
  },
  {
    path: ':id',
    component: EmailTemplateManageComponent,
    resolve: { emailTemplate: EmailTemplateResolver },
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EmailTemplateRoutingModule { }
