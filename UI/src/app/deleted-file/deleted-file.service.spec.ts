import { TestBed } from '@angular/core/testing';

import { DeletedFileService } from './deleted-file.service';

describe('DeletedFileService', () => {
  let service: DeletedFileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeletedFileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
