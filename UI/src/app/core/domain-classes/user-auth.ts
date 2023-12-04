import { Claim } from './claim';
export class UserAuth {
  id?: string;
  userName: string = '';
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  phoneNumber: string = '';
  bearerToken: string = '';
  isAuthenticated: boolean = false;
  profilePhoto?: string;
  isAdmin: boolean;
  claims?: Claim[]
}
