import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentMoveComponent } from './document-move.component';

describe('DocumentMoveComponent', () => {
  let component: DocumentMoveComponent;
  let fixture: ComponentFixture<DocumentMoveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentMoveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentMoveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
