import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CopyFolderComponent } from './copy-folder.component';

describe('CopyFolderComponent', () => {
  let component: CopyFolderComponent;
  let fixture: ComponentFixture<CopyFolderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CopyFolderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CopyFolderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
