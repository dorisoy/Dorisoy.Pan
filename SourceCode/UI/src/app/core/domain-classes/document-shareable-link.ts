export class DocumentShareableLink {
    id?: string;
    documentId: string;
    documentName?: string;
    linkExpiryTime?: Date;
    password: string;
    linkCode: string;
    isLinkExpired?: boolean;
    isAllowDownload: boolean;
    hasPassword?: boolean;
    extension?: string;
}