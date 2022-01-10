import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { HierarchyShared } from '@core/domain-classes/hierarchy-shared';

@Component({
  selector: 'app-prevent-shared-folder',
  templateUrl: './prevent-shared-folder.component.html',
  styleUrls: ['./prevent-shared-folder.component.scss']
})
export class PreventSharedFolderComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<PreventSharedFolderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: HierarchyShared,
  ) { }

  ngOnInit(): void {
  }

  onCancel() {
    this.dialogRef.close();
  }

}
