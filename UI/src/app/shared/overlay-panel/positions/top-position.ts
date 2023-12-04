import {ConnectedPosition} from '@angular/cdk/overlay';

export const TOP_POSITION: ConnectedPosition[] = [
    {originX: 'center', originY: 'top', overlayX: 'center', overlayY: 'bottom'},
    {originX: 'center', originY: 'bottom', overlayX: 'center', overlayY: 'top'},
];
