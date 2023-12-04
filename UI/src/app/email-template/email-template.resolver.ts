import { Injectable } from '@angular/core';
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot
} from '@angular/router';
import { EmailTemplate } from '@core/domain-classes/email-template';
import { Observable } from 'rxjs';
import { EmailTemplateService } from './email-template.service';

@Injectable(
  {
    providedIn: 'root'
  }
)
export class EmailTemplateResolver implements Resolve<EmailTemplate> {
  constructor(private emailTemplateService: EmailTemplateService) { }
  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<EmailTemplate> {
    const id = route.paramMap.get('id');
    if (id === 'add') {
      return null;
    }
    return this.emailTemplateService.getEmailTemplate(id) as Observable<EmailTemplate>;
  }
}
