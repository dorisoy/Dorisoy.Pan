import { TestBed } from '@angular/core/testing';

import { SharedFileService } from './shared-file.service';

describe('SharedFileService', () => {
  let service: SharedFileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SharedFileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
