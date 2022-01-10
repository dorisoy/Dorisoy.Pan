import {ScrollStrategy} from '@angular/cdk/overlay';

export class FullscreenOverlayScrollStrategy implements ScrollStrategy {
    public attach() {}

    public enable() {
        document.documentElement.classList.add('be-fullscreen-overlay-scrollblock');
    }

    public disable() {
        document.documentElement.classList.remove('be-fullscreen-overlay-scrollblock');
    }
}
