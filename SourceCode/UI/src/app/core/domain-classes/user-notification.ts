export class Notification {
    id: string;
    documentId?: string;
    folderId?: string;
    folderName: string;
    documentName: string;
    extension: string;
    documentThumbnail: string;
    createdDate: Date;
    fromUserName: string;
    fromUserId: string;
    isRead: boolean;
}
