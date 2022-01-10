import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailSmtpSettingListComponent } from './email-smtp-setting-list.component';

describe('EmailSmtpSettingListComponent', () => {
  let component: EmailSmtpSettingListComponent;
  let fixture: ComponentFixture<EmailSmtpSettingListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmailSmtpSettingListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmailSmtpSettingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
