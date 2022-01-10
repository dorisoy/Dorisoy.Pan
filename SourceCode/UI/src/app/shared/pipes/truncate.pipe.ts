import { Pipe } from '@angular/core';

// tslint:disable-next-line:use-pipe-transform-interface
@Pipe({
    name: 'limitTo'
})
export class TruncatePipe {
    transform(value: string, args: string): string {
        if (!value)
            return '';
        const limit = args ? parseInt(args, 10) : 100;
        const trail = '...';
        return value.length > limit ? value.substring(0, limit) + trail : value;
    }
}
