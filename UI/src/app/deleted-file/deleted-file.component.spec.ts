import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeletedFileComponent } from './deleted-file.component';

describe('DeletedFileComponent', () => {
  let component: DeletedFileComponent;
  let fixture: ComponentFixture<DeletedFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeletedFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeletedFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
