import { Documents } from './document';
import { User } from './user';

export interface RecentActivity {
  id?: string;
  createdDate?: Date;
  name?: string;
  isShared?: boolean;
  folderId?: string;
  documentId?: string;
  action: RecentActivityType;
  thumbnailPath?: string;
  document?: Documents;
  users?: User[];
}
export enum RecentActivityType {
  VIEWED,
  CREATED
}
