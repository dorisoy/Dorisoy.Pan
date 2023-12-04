import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailTemplateManageComponent } from './email-template-manage.component';

describe('EmailTemplateManageComponent', () => {
  let component: EmailTemplateManageComponent;
  let fixture: ComponentFixture<EmailTemplateManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmailTemplateManageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmailTemplateManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
