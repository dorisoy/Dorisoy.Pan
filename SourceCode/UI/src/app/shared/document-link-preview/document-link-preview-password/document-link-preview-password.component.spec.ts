import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentLinkPreviewPasswordComponent } from './document-link-preview-password.component';

describe('DocumentLinkPreviewPasswordComponent', () => {
  let component: DocumentLinkPreviewPasswordComponent;
  let fixture: ComponentFixture<DocumentLinkPreviewPasswordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentLinkPreviewPasswordComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentLinkPreviewPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
