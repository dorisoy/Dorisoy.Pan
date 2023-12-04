import { TestBed } from '@angular/core/testing';

import { DocumentShareableLinkService } from './document-shareable-link.service';

describe('DocumentShareableLinkService', () => {
  let service: DocumentShareableLinkService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DocumentShareableLinkService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
