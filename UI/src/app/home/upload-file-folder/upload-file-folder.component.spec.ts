import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadFileFolderComponent } from './upload-file-folder.component';

describe('UploadFileFolderComponent', () => {
  let component: UploadFileFolderComponent;
  let fixture: ComponentFixture<UploadFileFolderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadFileFolderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadFileFolderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
