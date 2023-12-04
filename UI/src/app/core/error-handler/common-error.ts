export interface CommonError {
  code?: number;
  statusText: string;
  messages: Array<string>;
  friendlyMessage: string;
  error?: { [key: string]: string } | string;
}
