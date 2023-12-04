import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmailSmtpSettingListComponent } from './email-smtp-setting-list/email-smtp-setting-list.component';
import { AuthGuard } from '@core/security/auth.guard';
import { ManageEmailSmtpSettingComponent } from './manage-email-smtp-setting/manage-email-smtp-setting.component';
import { EmailSMTPSettingDetailResolver } from './email-settting-detail.resolver';

const routes: Routes = [
  {
    path: '',
    component: EmailSmtpSettingListComponent,
    canActivate: [AuthGuard]
  }, {
    path: 'manage/:id',
    component: ManageEmailSmtpSettingComponent,
    resolve: { smtpSetting: EmailSMTPSettingDetailResolver },
    canActivate: [AuthGuard]
  }, {
    path: 'manage',
    component: ManageEmailSmtpSettingComponent,
    canActivate: [AuthGuard]
  },
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EmailSmtpSettingRoutingModule { }
