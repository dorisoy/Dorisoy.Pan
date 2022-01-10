import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RenameFileFolerComponent } from './rename-file-foler.component';

describe('RenameFileFolerComponent', () => {
  let component: RenameFileFolerComponent;
  let fixture: ComponentFixture<RenameFileFolerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RenameFileFolerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RenameFileFolerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
