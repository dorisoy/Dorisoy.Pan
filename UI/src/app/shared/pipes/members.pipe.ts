import { Pipe } from '@angular/core';

// tslint:disable-next-line:use-pipe-transform-interface
@Pipe({
  name: 'members'
})
export class MembersPipe {
  transform(value: number): string {
    return value === 1 ? '仅自己' : `${value} 可见成员`;
  }
}
