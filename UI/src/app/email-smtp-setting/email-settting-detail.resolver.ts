import { Injectable } from '@angular/core';
import {
    Resolve,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';
import { EmailSMTPSetting } from '@core/domain-classes/email-smtp-setting';
import { Observable } from 'rxjs';
import { EmailSmtpSettingService } from './email-smtp-setting.service';

@Injectable()
export class EmailSMTPSettingDetailResolver implements Resolve<EmailSMTPSetting> {
    constructor(private emailSmtpSettingService: EmailSmtpSettingService) { }
    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<EmailSMTPSetting> {
        const id = route.paramMap.get('id');
        return this.emailSmtpSettingService.getEmailSMTPSetting(id) as Observable<EmailSMTPSetting>;
    }
}