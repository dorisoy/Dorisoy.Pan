import { Injectable } from '@angular/core'
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { CommonDialogComponent } from './common-dialog.component';
@Injectable()
export class CommonDialogService {
    dialogConfig: MatDialogConfig = {
        disableClose: false,
        width: '',
        height: '',
        position: {
            top: '',
            bottom: '',
            left: '',
            right: ''
        }
    };
    constructor(public dialog: MatDialog) { }

    deleteConformationDialog(message: string): Observable<boolean> {
        const dialogRef = this.dialog.open(CommonDialogComponent, this.dialogConfig);
        dialogRef.componentInstance.primaryMessage = message;
        return dialogRef.afterClosed();
    }
}
