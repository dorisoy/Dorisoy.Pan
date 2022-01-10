import { NgModule } from '@angular/core';
import { PipesModule } from './pipes/pipes.module';
import { DragDropDirective } from './directives/drag-drop.directive';
import { CommonModule } from '@angular/common';
import { DocumentListComponent } from './document-list/document-list.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { ReactiveFormsModule } from '@angular/forms';
import { PdfViewerComponent } from './pdf-viewer/pdf-viewer.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { OfficeViewerComponent } from './office-viewer/office-viewer.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { OverlayModule } from '@angular/cdk/overlay';
import { FilePreviewToolbarComponent } from './file-preview-toolbar/file-preview-toolbar.component';
import { ImagePreviewComponent } from './image-preview/image-preview.component';
import { TextPreviewComponent } from './text-preview/text-preview.component';
import { AudioPreviewComponent } from './audio-preview/audio-preview.component';
import { VideoPreviewComponent } from './video-preview/video-preview.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FileUploadProcessComponent } from './file-upload-process/file-upload-process.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { SearchComponent } from './search/search.component';
import { DocumentBaseComponent } from './document-base/document-base.component';
import { DocumentLinkPreviewComponent } from './document-link-preview/document-link-preview.component';
import { DocumentLinkPreviewPasswordComponent } from './document-link-preview/document-link-preview-password/document-link-preview-password.component';
import { RouterModule } from '@angular/router';
import { DocumentVersionHistoryComponent } from './document-version-history/document-version-history.component';
import { NgxFilesizeModule } from 'ngx-filesize';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NoPreviewAvailableComponent } from './no-preview-available/no-preview-available.component';
import { RenameFileFolderComponent } from './rename-file-folder/rename-file-folder.component';
import { SendMailComponent } from './send-mail/send-mail.component';
import { NgxEditorModule } from 'ngx-editor';
import { BasePreviewComponent } from './base-preview/base-preview.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime-ex';
import { HasClaimDirective } from './has-claim.directive';

@NgModule({
  exports: [
    PipesModule,
    DragDropDirective,
    DocumentListComponent,
    OverlayModule,
    MatProgressSpinnerModule,
    HasClaimDirective
  ],
  imports: [
    CommonModule,
    PipesModule,
    ReactiveFormsModule,
    RouterModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatDialogModule,
    MatAutocompleteModule,
    NgxExtendedPdfViewerModule,
    NgxDocViewerModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatExpansionModule,
    MatListModule,
    MatProgressBarModule,
    NgxFilesizeModule,
    DragDropModule,
    NgxEditorModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
  ],
  declarations: [
    DragDropDirective,
    DocumentListComponent,
    FilePreviewToolbarComponent,
    PdfViewerComponent,
    OfficeViewerComponent,
    ImagePreviewComponent,
    TextPreviewComponent,
    AudioPreviewComponent,
    VideoPreviewComponent,
    FileUploadProcessComponent,
    SearchComponent,
    DocumentBaseComponent,
    DocumentLinkPreviewComponent,
    DocumentLinkPreviewPasswordComponent,
    DocumentVersionHistoryComponent,
    RenameFileFolderComponent,
    NoPreviewAvailableComponent,
    SendMailComponent,
    BasePreviewComponent,
    HasClaimDirective],
  providers: [
    { provide: MAT_DIALOG_DATA, useValue: {} },
    { provide: MatDialogRef, useValue: {} }
  ]
})
export class SharedModule { }
