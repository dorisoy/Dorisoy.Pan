import { Documents } from './document';
import { Folder } from './folder';

export interface SharedData {
  folder?: Folder;
  document?: Documents;
  type: string;
  parentPhysicalFolderId: string;
}
