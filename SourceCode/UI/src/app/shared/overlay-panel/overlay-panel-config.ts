import {ElementRef, ViewContainerRef} from '@angular/core';
import { ConnectedPosition, PositionStrategy } from '@angular/cdk/overlay';

export interface OverlayPanelConfig {
    scrollStrategy?: string;
    position?: OverlayPanelPosition;
    mobilePosition?: OverlayPanelPosition;
    positionStrategy?: PositionStrategy;
    origin?: ElementRef | 'global';
    hasBackdrop?: boolean;
    closeOnBackdropClick?: boolean;
    panelClass?: string | string[];
    backdropClass?: string | string[];
    fullScreen?: boolean;
    data?: any;
    width?: number | string;
    height?: number | string;
    maxWidth?: number | string;
    maxHeight?: number | string;
    viewContainerRef?: ViewContainerRef;
}

export interface OverlayPanelGlobalPosition {
    top?: string|number;
    bottom?: string|number;
    right?: string|number;
    left?: string|number;
}

export type OverlayPanelPosition = ConnectedPosition[]|OverlayPanelGlobalPosition|'center';
