import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentBaseComponent } from './document-base.component';

describe('DocumentBaseComponent', () => {
  let component: DocumentBaseComponent;
  let fixture: ComponentFixture<DocumentBaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentBaseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentBaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
