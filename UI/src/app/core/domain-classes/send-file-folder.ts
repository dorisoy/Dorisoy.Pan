import { Documents } from "./document";
import { Folder } from "./folder";

export class SendFileFolderData {
    type: string;
    folder?: Folder;
    document?: Documents;
}