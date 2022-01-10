import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageEmailSmtpSettingComponent } from './manage-email-smtp-setting.component';

describe('ManageEmailSmtpSettingComponent', () => {
  let component: ManageEmailSmtpSettingComponent;
  let fixture: ComponentFixture<ManageEmailSmtpSettingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageEmailSmtpSettingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageEmailSmtpSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
