import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PreventSharedFolderComponent } from './prevent-shared-folder.component';

describe('PreventSharedFolderComponent', () => {
  let component: PreventSharedFolderComponent;
  let fixture: ComponentFixture<PreventSharedFolderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PreventSharedFolderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PreventSharedFolderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
