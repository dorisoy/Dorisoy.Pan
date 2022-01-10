import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FolderPathComponent } from './folder-path.component';

describe('FolderPathComponent', () => {
  let component: FolderPathComponent;
  let fixture: ComponentFixture<FolderPathComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FolderPathComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FolderPathComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
