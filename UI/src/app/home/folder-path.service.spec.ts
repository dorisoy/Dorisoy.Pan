import { TestBed } from '@angular/core/testing';

import { ObservableService } from '../core/services/observable.service';

describe('FolderPathService', () => {
  let service: ObservableService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ObservableService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
