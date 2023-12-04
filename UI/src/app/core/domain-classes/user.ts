export interface User {
  id?: string;
  userName: string;
  email: string;
  firstName?: string;
  lastName?: string;
  password?: string;
  phoneNumber?: string;
  profilePhoto?: string;
  address?: string;
  isActive?: boolean;
  isAdmin?: boolean;
  isProfilePhotoChanged?: boolean;
  provider?: string;
  latitude?: number;
  longitude?: number;
  isOwner?: boolean;
  userClaims?: UserClaim;
  size?: number;
}

export interface UserClaim{
  isFolderCreate?:boolean,
  isFileUpload?:boolean,
  isDeleteFileFolder?:boolean,
  isSharedFileFolder?:boolean,
  isSendEmail?:boolean,
  isRenameFile?:boolean,
  isDownloadFile?:boolean,
  isCopyFile?:boolean,
  isCopyFolder?:boolean,
  isMoveFile?:boolean,
  isSharedLink?:boolean
}
