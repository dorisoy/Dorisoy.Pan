export class FolderPath {
    id: string;
    name: string;
    parentId: string;
    physicalFolderId?: string;
    level: number = 0;
}