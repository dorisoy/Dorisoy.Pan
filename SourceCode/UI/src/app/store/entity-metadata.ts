import { EntityMetadataMap } from '@ngrx/data';

const entityMetadata: EntityMetadataMap = {
  Page: {
  },
  Action: {
  },
  PageAction: {
    entityDispatcherOptions: {
      optimisticDelete: true
    }
  }
};

export const entityConfig = {
  entityMetadata
};
