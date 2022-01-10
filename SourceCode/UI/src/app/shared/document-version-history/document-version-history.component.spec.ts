import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentVersionHistoryComponent } from './document-version-history.component';

describe('DocumentVersionHistoryComponent', () => {
  let component: DocumentVersionHistoryComponent;
  let fixture: ComponentFixture<DocumentVersionHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentVersionHistoryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentVersionHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
