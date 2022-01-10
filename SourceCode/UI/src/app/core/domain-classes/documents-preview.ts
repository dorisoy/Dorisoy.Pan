import { Documents } from "./document";
import { Folder } from './folder';

export class DocumentsPreview {
    document: Documents;
    otherDocuments?: Documents[];
    selectFolder?: Folder;
    rootFolder?: Folder;
}
