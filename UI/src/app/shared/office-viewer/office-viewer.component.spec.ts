import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OfficeViewerComponent } from './office-viewer.component';

describe('OfficeViewerComponent', () => {
  let component: OfficeViewerComponent;
  let fixture: ComponentFixture<OfficeViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OfficeViewerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OfficeViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
