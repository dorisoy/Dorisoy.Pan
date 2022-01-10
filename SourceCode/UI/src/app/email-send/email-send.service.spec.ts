import { TestBed } from '@angular/core/testing';

import { EmailSendService } from './email-send.service';

describe('EmailSendService', () => {
  let service: EmailSendService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmailSendService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
