
import { Documents } from "./document";
import { User } from "./user";

export class Folder {
  id: string;
  name: string;
  parentId?: string;
  physicalFolderId?: string;
  children?: Folder[];
  documents?: Documents[];
  users?: User[];
  virtualParentId?: string;
  createdDate?: Date;
  isRestore?: boolean;
  isShared?: boolean;
  isStarred?: boolean;
  isRightClicked?:boolean;
}
