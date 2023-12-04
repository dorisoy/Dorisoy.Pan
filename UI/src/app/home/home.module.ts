import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { HomeRoutingModule } from './home-routing.module';
import { AddFolderComponent } from './add-folder/add-folder.component';
import { MatDialogModule } from '@angular/material/dialog';
import { SharedModule } from '@shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatChipsModule } from '@angular/material/chips';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { SharedFolderFileComponent } from './shared-folder-file/shared-folder-file.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FolderPathComponent } from './folder-path/folder-path.component';
import { UploadFileFolderComponent } from './upload-file-folder/upload-file-folder.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SharedFolderUsersComponent } from './shared-folder-file/shared-folder-users/shared-folder-users.component';
import { PreventSharedFolderComponent } from './shared-folder-file/prevent-shared-folder/prevent-shared-folder.component';
import { MoveFolderComponent } from './move-folder/move-folder.component';
import { CopyFolderComponent } from './copy-folder/copy-folder.component';
import { DocumentCommentComponent } from './document-comment/document-comment.component';
import { FolderListComponent } from './folder-list/folder-list.component';
import { DocumentCopyComponent } from './document-copy/document-copy.component';
import { DocumentMoveComponent } from './document-move/document-move.component';
import { ToolBarComponent } from './tool-bar/tool-bar.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { DocumentSharedLinkComponent } from './document-shared-link/document-shared-link.component';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { MatSortModule } from '@angular/material/sort';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime-ex';


@NgModule({
  declarations: [
    HomeComponent,
    AddFolderComponent,
    FolderPathComponent,
    SharedFolderFileComponent,
    UploadFileFolderComponent,
    SharedFolderUsersComponent,
    PreventSharedFolderComponent,
    MoveFolderComponent,
    CopyFolderComponent,
    DocumentCommentComponent,
    FolderListComponent,
    DocumentCopyComponent,
    DocumentMoveComponent,
    ToolBarComponent,
    DocumentSharedLinkComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HomeRoutingModule,
    SharedModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatDialogModule,
    MatChipsModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule,
    MatCheckboxModule,
    DragDropModule,
    MatDatepickerModule,
    MatNativeDateModule,
    ClipboardModule,
    MatSortModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
  ]
})
export class HomeModule { }
