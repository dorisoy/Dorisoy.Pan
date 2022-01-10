import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginAuditListComponent } from './login-audit-list.component';

describe('LoginAuditListComponent', () => {
  let component: LoginAuditListComponent;
  let fixture: ComponentFixture<LoginAuditListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LoginAuditListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginAuditListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
