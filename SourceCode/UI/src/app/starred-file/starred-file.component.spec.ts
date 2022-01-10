import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StarredFileComponent } from './starred-file.component';

describe('StarredFileComponent', () => {
  let component: StarredFileComponent;
  let fixture: ComponentFixture<StarredFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StarredFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StarredFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
