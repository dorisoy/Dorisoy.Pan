import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TruncatePipe } from './truncate.pipe';
import { DateAgoPipe } from './date-ago.pipe';
import { MembersPipe } from './members.pipe';
import { FileSizePipe } from './file-size.pipe';

@NgModule({
    declarations: [
        TruncatePipe,
        DateAgoPipe,
        MembersPipe,
        FileSizePipe
    ],
    imports: [
        CommonModule
    ],
    exports: [
        TruncatePipe,
        DateAgoPipe,
        MembersPipe,
        FileSizePipe
    ]
})
export class PipesModule { }
