// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
    apiUrl: 'http://localhost:8081/',
    officeDocumentExtensions: ['.doc', '.docx', '.ppt', '.pptx', '.xls', '.xlsx'],
    imageExtensions: ['.jpg', '.jpeg', '.png', '.gif', '.tiff', '.psd', '.bmp', '.webp', '.raw', '.bmp', '.heif', '.indd', '.svg', '.ai', '.eps'],
    audioFileExtension: ['.3gp', '.aa', '.aac', '.aax', '.act', '.aiff', '.alac', '.amr', '.ape', '.au', '.awb', '.dss', '.dvf', '.flac', '.gsm', '.iklx', '.ivs', '.m4a', '.m4b', '.m4p', '.mmf', '.mp3', '.mpc', '.msv', '.nmf', '.ogg', '.oga', '.mogg', '.opus', '.org', '.ra', '.rm', '.raw', '.rf64', '.sln', '.tta', '.voc', '.vox', '.wav', '.wma', '.wv', '.webm'],
    videoFileExtension: ['.webm', '.flv', '.vob', '.ogv', '.ogg', '.drc', '.avi', '.mts', '.m2ts', '.wmv', '.yuv', '.viv', '.mp4', '.m4p', '.3pg', '.flv', '.f4v', '.f4a']
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
