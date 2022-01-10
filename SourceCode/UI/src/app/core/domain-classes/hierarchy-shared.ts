export interface HierarchyShared {
  id: string;
  isParentShared: boolean;
  isChildShared: boolean;
  isFolderShared?: boolean;
  name?: string;
  operation: string;
  isFolder: boolean;
}
