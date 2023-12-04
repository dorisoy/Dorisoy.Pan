export class TreeViewFolder {
  id: string;
  name: string;
  parentId: string;
  level: number = 0;
  isLoading: boolean = false;
  expandable: boolean = false;
  physicalFolderId: string;
  isShared: boolean = false;
}
