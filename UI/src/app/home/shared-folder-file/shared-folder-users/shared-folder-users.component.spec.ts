import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedFolderUsersComponent } from './shared-folder-users.component';

describe('SharedFolderUsersComponent', () => {
  let component: SharedFolderUsersComponent;
  let fixture: ComponentFixture<SharedFolderUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SharedFolderUsersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedFolderUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
