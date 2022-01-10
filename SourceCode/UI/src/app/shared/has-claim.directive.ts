import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { SecurityService } from '../core/security/security.service';

@Directive({
  // tslint:disable-next-line: directive-selector
  selector: '[hasClaim]'
})
export class HasClaimDirective {
  @Input() set hasClaim(claimType: any) {
    if (this.securityService.hasClaim(claimType)) {
      // Add template to DOM
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      // Remove template from DOM
      this.viewContainer.clear();
    }
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private securityService: SecurityService) { }
}
