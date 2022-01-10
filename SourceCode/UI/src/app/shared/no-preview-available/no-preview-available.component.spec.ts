import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NoPreviewAvailableComponent } from './no-preview-available.component';

describe('NoPreviewAvailableComponent', () => {
  let component: NoPreviewAvailableComponent;
  let fixture: ComponentFixture<NoPreviewAvailableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NoPreviewAvailableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NoPreviewAvailableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
