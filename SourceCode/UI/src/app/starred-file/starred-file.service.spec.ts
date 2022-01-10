import { TestBed } from '@angular/core/testing';

import { StarredFileService } from './starred-file.service';

describe('StarredFileService', () => {
  let service: StarredFileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StarredFileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
