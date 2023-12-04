import { ResourceParameter } from './resource-parameter';

export class UserResource extends ResourceParameter {
  email: string = '';
  first_name: string = '';
  last_name: string = '';
  phone_number: string = '';
  is_active: boolean = true;
  folderId: string = '';
  documentId: string = '';
  physicalFolderId: string = '';
  type: string;
}
