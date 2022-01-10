export class LoginAudit {
    id?: string;
    userName: string;
    loginTime: Date;
    remoteIP: string;
    status: string;
    provider: string;
    latitude?: string;
    longitude?: string;
}