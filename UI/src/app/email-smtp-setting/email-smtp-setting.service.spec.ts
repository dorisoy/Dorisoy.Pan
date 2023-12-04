import { TestBed } from '@angular/core/testing';

import { EmailSmtpSettingService } from './email-smtp-setting.service';

describe('EmailSmtpSettingService', () => {
  let service: EmailSmtpSettingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmailSmtpSettingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
