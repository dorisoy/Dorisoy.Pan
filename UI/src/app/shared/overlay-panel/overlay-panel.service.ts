import { ElementRef, Injectable, Injector, TemplateRef } from '@angular/core';
import { ConnectedPosition, Overlay, OverlayConfig, PositionStrategy, ScrollStrategy } from '@angular/cdk/overlay';
import { ComponentPortal, ComponentType, PortalInjector, TemplatePortal } from '@angular/cdk/portal';
import { OverlayPanelRef } from './overlay-panel-ref';
import { OVERLAY_PANEL_DATA } from './overlay-panel-data';
import { OverlayPanelConfig, OverlayPanelPosition } from './overlay-panel-config';
import { FullscreenOverlayScrollStrategy } from './fullscreen-overlay-scroll-strategy';
import { filter } from 'rxjs/operators';
import { ESCAPE } from '@angular/cdk/keycodes';
import { BreakpointsService } from '@core/services/breakpoints.service';

const DEFAULT_CONFIG = {
    hasBackdrop: true,
    closeOnBackdropClick: true,
    panelClass: 'overlay-panel',
};

@Injectable({
    providedIn: 'root'
})
export class OverlayPanel {
    constructor(
        public overlay: Overlay,
        private breakpoints: BreakpointsService,
        private injector: Injector,
    ) { }

    public open<T>(cmp: ComponentType<T> | TemplateRef<any>, userConfig: OverlayPanelConfig): OverlayPanelRef<T> {
        const config = Object.assign({}, DEFAULT_CONFIG, userConfig);
        const cdkConfig = {
            positionStrategy: this.getPositionStrategy(config),
            hasBackdrop: config.hasBackdrop,
            panelClass: config.panelClass,
            backdropClass: config.backdropClass,
            scrollStrategy: this.getScrollStrategy(config),
            disposeOnNavigation: true,
        } as OverlayConfig;

        if (config.width) cdkConfig.width = config.width;
        if (config.height) cdkConfig.height = config.height;
        if (config.maxHeight) cdkConfig.maxHeight = config.maxHeight;
        if (config.maxWidth) cdkConfig.maxWidth = config.maxWidth;

        const overlayRef = this.overlay.create(cdkConfig);
        const overlayPanelRef = new OverlayPanelRef<T>(overlayRef);
        const portal = cmp instanceof TemplateRef ?
            new TemplatePortal(cmp, config.viewContainerRef, config.data) :
            new ComponentPortal(cmp, config.viewContainerRef, this.createInjector(config, overlayPanelRef));
        overlayPanelRef.componentRef = overlayRef.attach(portal);

        if (config.closeOnBackdropClick) {
            overlayRef.backdropClick().subscribe(() => overlayPanelRef.close());
            overlayRef.keydownEvents()
                .pipe(filter(e => e.keyCode === ESCAPE))
                .subscribe(() => overlayPanelRef.close());
        }

        return overlayPanelRef;
    }

    private getScrollStrategy(config: OverlayPanelConfig): ScrollStrategy {
        if (config.fullScreen) {
            return new FullscreenOverlayScrollStrategy();
        } else if (config.scrollStrategy === 'close') {
            return this.overlay.scrollStrategies.close();
        } else {
            return null;
        }
    }

    private createInjector(config: OverlayPanelConfig, dialogRef: OverlayPanelRef<any>): PortalInjector {
        const injectionTokens = new WeakMap();
        injectionTokens.set(OverlayPanelRef, dialogRef);
        injectionTokens.set(OVERLAY_PANEL_DATA, config.data || null);
        return new PortalInjector(this.injector, injectionTokens);
    }

    private getPositionStrategy(config: OverlayPanelConfig) {
        if (config.positionStrategy) {
            return config.positionStrategy;
        }

        const position = this.breakpoints.isMobile$.value ?
            (config.mobilePosition || config.position) :
            config.position;
        if (config.origin === 'global' || this.positionIsGlobal(position)) {
            return this.getGlobalPositionStrategy(position);
        } else {
            return this.getConnectedPositionStrategy(position, config.origin);
        }
    }

    private positionIsGlobal(position: OverlayPanelPosition) {
        return position === 'center' || !Array.isArray(position);
    }

    private getGlobalPositionStrategy(position: OverlayPanelPosition): PositionStrategy {
        if (position === 'center') {
            return this.overlay.position().global().centerHorizontally().centerVertically();
        } else {
            const global = this.overlay.position().global();
            Object.keys(position).forEach(key => {
                global[key](position[key]);
            });
            return global;
        }
    }

    private getConnectedPositionStrategy(position: OverlayPanelPosition, origin: ElementRef) {
        return this.overlay.position()
            .flexibleConnectedTo(origin)
            .withPositions(position as ConnectedPosition[])
            .withPush(true)
            .withViewportMargin(5);
    }
}
