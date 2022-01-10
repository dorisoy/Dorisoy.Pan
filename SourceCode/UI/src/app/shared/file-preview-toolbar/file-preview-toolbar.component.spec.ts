import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilePreviewToolbarComponent } from './file-preview-toolbar.component';

describe('FilePreviewToolbarComponent', () => {
  let component: FilePreviewToolbarComponent;
  let fixture: ComponentFixture<FilePreviewToolbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilePreviewToolbarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FilePreviewToolbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
