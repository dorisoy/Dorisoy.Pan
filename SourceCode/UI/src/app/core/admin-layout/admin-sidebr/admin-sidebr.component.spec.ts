import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminSidebrComponent } from './admin-sidebr.component';

describe('AdminSidebrComponent', () => {
  let component: AdminSidebrComponent;
  let fixture: ComponentFixture<AdminSidebrComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminSidebrComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminSidebrComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
