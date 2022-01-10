import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedFolderFileComponent } from './shared-folder-file.component';

describe('SharedFolderFileComponent', () => {
  let component: SharedFolderFileComponent;
  let fixture: ComponentFixture<SharedFolderFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SharedFolderFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedFolderFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
