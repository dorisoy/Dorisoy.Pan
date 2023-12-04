import {ComponentType, OverlayRef} from '@angular/cdk/overlay';
import {Observable, Subject} from 'rxjs';
import {take} from 'rxjs/operators';
import {ComponentRef} from '@angular/core';
import { randomString } from '@core/utils/random-string';

export class OverlayPanelRef<T = ComponentType<any>, V = any> {
    public id: string = randomString(15);
    private value = new Subject<any>();
    public componentRef: ComponentRef<T>;

    constructor(public overlayRef: OverlayRef) {}

    public isOpen(): boolean {
        return this.overlayRef && this.overlayRef.hasAttached();
    }

    public close() {
        this.overlayRef && this.overlayRef.dispose();
    }

    public emitValue(value: V) {
        this.value.next(value);
    }

    public valueChanged(): Observable<V> {
        return this.value.asObservable();
    }

    public getPanelEl() {
        return this.overlayRef.overlayElement;
    }

    public updatePosition() {
        return this.overlayRef.updatePosition();
    }

    public afterClosed() {
        return this.overlayRef.detachments().pipe(take(1));
    }
}
