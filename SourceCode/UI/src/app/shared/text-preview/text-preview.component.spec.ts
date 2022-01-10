import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TextPreviewComponent } from './text-preview.component';

describe('TextPreviewComponent', () => {
  let component: TextPreviewComponent;
  let fixture: ComponentFixture<TextPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TextPreviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TextPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
