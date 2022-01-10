import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentSharedLinkComponent } from './document-shared-link.component';

describe('DocumentSharedLinkComponent', () => {
  let component: DocumentSharedLinkComponent;
  let fixture: ComponentFixture<DocumentSharedLinkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentSharedLinkComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentSharedLinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
