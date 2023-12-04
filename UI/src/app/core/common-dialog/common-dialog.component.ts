import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-common-dialog',
  templateUrl: './common-dialog.component.html',
  styleUrls: ['./common-dialog.component.scss']
})
export class CommonDialogComponent {
  primaryMessage: string;
  constructor(public dialogRef: MatDialogRef<CommonDialogComponent>) { }

  clickHandler(data): void {
    this.dialogRef.close(data);
  }

}
