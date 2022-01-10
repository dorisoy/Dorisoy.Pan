import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminLayoutComponent } from '@core/admin-layout/admin-layout.component';
import { AdminAuthGuard } from '@core/security/admin-auth.guard';
import { AuthGuard } from '@core/security/auth.guard';
import { DocumentLinkPreviewComponent } from '@shared/document-link-preview/document-link-preview.component';
import { LayoutComponent } from './core/layout/layout.component';
import { RootResolver } from './core/services/root-resolver';
import { EmptyComponent } from './empty/empty.component';
import { MyProfileComponent } from './user/my-profile/my-profile.component';

const routes: Routes = [
  {
    path: 'preview/:code',
    component: DocumentLinkPreviewComponent,
  }, {
    path: 'login',
    loadChildren: () =>
      import('./login/login.module')
        .then(m => m.LoginModule)
  }, {
    path: 'admin',
    component: AdminLayoutComponent,
    children: [
      {
        path: '',
        canLoad: [AdminAuthGuard],
        loadChildren: () =>
          import('./user/user.module')
            .then(m => m.UserModule)
      }, {
        path: 'login-audit',
        canLoad: [AdminAuthGuard],
        loadChildren: () =>
          import('./login-audit/login-audit.module')
            .then(m => m.LoginAuditModule)
      }, {
        path: 'dashboard',
        canLoad: [AdminAuthGuard],
        loadChildren: () =>
          import('./dashboard/dashboard.module')
            .then(m => m.DashboardModule)
      }, {
        path: 'email-template',
        canLoad: [AdminAuthGuard],
        loadChildren: () =>
          import('./email-template/email-template.module')
            .then(m => m.EmailTemplateModule)
      }, {
        path: 'send-email',
        canLoad: [AdminAuthGuard],
        loadChildren: () =>
          import('./email-send/email-send.module')
            .then(m => m.EmailSendModule)
      }, {
        path: 'logs',
        canLoad: [AdminAuthGuard],
        loadChildren: () =>
          import('./n-log/n-log.module')
            .then(m => m.NLogModule)
      }, {
        path: 'email-smtp',
        canLoad: [AdminAuthGuard],
        loadChildren: () =>
          import('./email-smtp-setting/email-smtp-setting.module')
            .then(m => m.EmailSmtpSettingModule)
      }
    ]
  },
  {
    path: '',
    component: LayoutComponent,
    resolve: { folder: RootResolver },
    children: [
      {
        path: '',
        component: EmptyComponent
      }, {
        path: 'my-profile',
        component: MyProfileComponent,
        canActivate: [AuthGuard],
      }, {
        path: 'deleted-files',
        canLoad: [AuthGuard],
        loadChildren: () =>
          import('./deleted-file/deleted-file.module')
            .then(m => m.DeletedFileModule)
      }, {
        path: 'shared-files',
        canLoad: [AuthGuard],
        loadChildren: () =>
          import('./shared-file/shared-file.module')
            .then(m => m.SharedFileModule)
      }, {
        path: 'starred-files',
        canLoad: [AuthGuard],
        loadChildren: () =>
          import('./starred-file/starred-file.module')
            .then(m => m.StarredFileModule)
      },
      {
        path: 'recents',
        canLoad: [AuthGuard],
        loadChildren: () =>
          import('./recent-activity/recent-activity.module')
            .then(m => m.RecentActivityModule)
      },
      {
        path: 'notifications',
        canLoad: [AuthGuard],
        loadChildren: () =>
          import('./notification/notification.module')
            .then(m => m.NotificationModule)
      },
      {
        path: ':id',
        canLoad: [AuthGuard],
        loadChildren: () =>
          import('./home/home.module')
            .then(m => m.HomeModule)
      },
      {
        path: '**',
        redirectTo: '/'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes,
    { scrollPositionRestoration: 'top', relativeLinkResolution: 'legacy', })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
