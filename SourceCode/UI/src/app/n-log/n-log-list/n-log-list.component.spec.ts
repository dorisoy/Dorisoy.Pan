import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NLogListComponent } from './n-log-list.component';

describe('NLogListComponent', () => {
  let component: NLogListComponent;
  let fixture: ComponentFixture<NLogListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NLogListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NLogListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
