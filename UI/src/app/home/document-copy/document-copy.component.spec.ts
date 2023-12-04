import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentCopyComponent } from './document-copy.component';

describe('DocumentCopyComponent', () => {
  let component: DocumentCopyComponent;
  let fixture: ComponentFixture<DocumentCopyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentCopyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentCopyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
