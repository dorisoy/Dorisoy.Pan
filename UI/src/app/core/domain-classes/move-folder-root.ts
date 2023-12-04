import { Folder } from './folder';

export interface MoveFolderRoot {
  root: Folder;
  sourceId: string;
  sourceName: string;
  sourceParentId?: string;
}
