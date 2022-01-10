import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NLogDetailComponent } from './n-log-detail.component';

describe('NLogDetailComponent', () => {
  let component: NLogDetailComponent;
  let fixture: ComponentFixture<NLogDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NLogDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NLogDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
