import { Injectable } from "@angular/core";
import { MatPaginatorIntl } from "@angular/material/paginator";

@Injectable() 
export class MatPaginatorIntlCN extends MatPaginatorIntl { 
    itemsPerPageLabel = '每页显示';
    nextPageLabel = '下一页';
    previousPageLabel = '上一页';
    firstPageLabel = '首页';
    lastPageLabel = '尾页';

    getRangeLabel = function (page: number, pageSize: number, length: number) {
        if (length === 0 || pageSize === 0) {
            return '0 到 ' + length;
        }
        length = Math.max(length, 0);
        const startIndex = page * pageSize;
        const endIndex = startIndex < length ?
            Math.min(startIndex + pageSize, length) :
            startIndex + pageSize;
        return startIndex + 1 + ' - ' + endIndex + ' / ' + length;
    };
}