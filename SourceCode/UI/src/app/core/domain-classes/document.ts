import { DocumentComment } from "./document-comment";
import { DocumentVersion } from "./document-version";
import { User } from "./user";

export class Documents {
  id: string;
  name: string;
  physicalFolderId: string;
  extension: string;
  path: string;
  size: string;
  modifiedDate: Date;
  thumbnailPath: string;
  isStarred?: boolean;
  users?: User[];
  isBackDisabled?: boolean = false;
  isVersion?: boolean;
  isDownloadEnabled?: boolean = false;
  isRightClicked?: boolean = false;
  token?: string;
  isFromPreview?: boolean;
  documentVersions?: DocumentVersion[];
  documentComments?: DocumentComment[];
}
