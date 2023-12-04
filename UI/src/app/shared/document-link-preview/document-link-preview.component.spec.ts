import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentLinkPreviewComponent } from './document-link-preview.component';

describe('DocumentLinkPreviewComponent', () => {
  let component: DocumentLinkPreviewComponent;
  let fixture: ComponentFixture<DocumentLinkPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentLinkPreviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentLinkPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
