using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.API.Controllers.Folder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FolderController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly PathHelper _pathHelper;

        public FolderController(IMediator mediator,
            PathHelper pathHelper)
        {
            _mediator = mediator;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Gets the folders.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        //[HttpGet("{id}", Name = "GetFolders")]
        //public async Task<IActionResult> GetFolders(Guid id)
        //{
        //    var getActionQuery = new GetVirtualFoldersQuery { Id = id };
        //    var result = await _mediator.Send(getActionQuery);
        //    return Ok(result);
        //}

        /// <summary>
        /// Get Folder Parents by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/parents")]
        public async Task<IActionResult> GetFolderParentsById(Guid id)
        {
            var getActionQuery = new GetFolderParentsByIdQuery { Id = id };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }


        [HttpGet("totalSize")]
        public async Task<IActionResult> GetTotalSizes()
        {
            var getTotalSizeOfFilesQuery = new GetTotalSizeOfFilesQuery { };
            var result = await _mediator.Send(getTotalSizeOfFilesQuery);
            return Ok(result);
        }

        [HttpPost("move")]
        public async Task<IActionResult> MoveFolder(MoveFolderCommand moveFolderCommand)
        {
            var result = await _mediator.Send(moveFolderCommand);
            return Ok(result);
        }

        [HttpPost("copy")]
        public async Task<IActionResult> CopyFolder(CopyFolderCommand copyFolderCommand)
        {
            var result = await _mediator.Send(copyFolderCommand);
            return Ok(result);
        }
        /// <summary>
        /// Uploads the documents.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadDocuments(Guid id)
        {
            var updateUserProfilePhotoCommand = new UploadDocumentCommand()
            {
                FolderId = id,
                Documents = Request.Form.Files,
            };
            if (Request.Form.TryGetValue("FullPath", out var fullName))
            {
                updateUserProfilePhotoCommand.FullPath = fullName;
            }
            var result = await _mediator.Send(updateUserProfilePhotoCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Creates the folders.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="addChildFoldersCommand">The add child folders command.</param>
        /// <returns></returns>
        [HttpPost("folder/{id}")]
        public async Task<IActionResult> CreateFolders(Guid id, AddChildFoldersCommand addChildFoldersCommand)
        {
            addChildFoldersCommand.VirtualFolderId = id;
            var result = await _mediator.Send(addChildFoldersCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Downloads the folder.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFolder(Guid id)
        {
            var command = new DownloadFolderCommand { Id = id };
            var listOfDocuments = await _mediator.Send(command);
            if (!listOfDocuments.Any())
            {
                return StatusCode(404, new List<string> { "Folder is empty" });
            }
            using var ms = new MemoryStream();
            using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var attachment in listOfDocuments)
                {
                    var path = Path.Combine(attachment.Path.Split(Path.DirectorySeparatorChar).SkipLast(1).Select(c => c).ToArray());
                    path = Path.Combine(path, attachment.Name);
                    var entry = zipArchive.CreateEntry(path);
                    //using var fileStream = new FileStream(attachment.OriginalFolderPath, FileMode.Open);
                    //using var entryStream = entry.Open();
                    //await fileStream.CopyToAsync(entryStream);

                    byte[] newBytes;
                    using (var stream = new FileStream(attachment.OriginalFolderPath, FileMode.Open))
                    {
                        using (var entryStream = entry.Open())
                        {

                            byte[] bytes = new byte[stream.Length];
                            int numBytesToRead = (int)stream.Length;
                            int numBytesRead = 0;
                            while (numBytesToRead > 0)
                            {
                                // Read may return anything from 0 to numBytesToRead.
                                int n = stream.Read(bytes, numBytesRead, numBytesToRead);

                                // Break when the end of the file is reached.
                                if (n == 0)
                                    break;

                                numBytesRead += n;
                                numBytesToRead -= n;
                            }
                            newBytes = AesOperation.DecryptStream(bytes, _pathHelper.EncryptionKey);
                            entryStream.Write(newBytes);
                        }
                    }
                }
            }
            ms.Position = 0;
            return File(ms.ToArray(), "application/zip");
        }

        /// <summary>
        /// Downloads the files and folders.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost("DownloadFilesAndFolders")]
        public async Task<IActionResult> DownloadFilesAndFolders(DownloadDocumentAndFolderCommand command)
        {
            var listOfDocuments = await _mediator.Send(command);
            if (!listOfDocuments.Any())
            {
                return StatusCode(404, new List<string> { "no file/folder selected." });
            }
            using var msFinal = new MemoryStream();
            using (var zipArchive = new ZipArchive(msFinal, ZipArchiveMode.Create, true))
            {
                foreach (var attachment in listOfDocuments)
                {
                    string path = attachment.Name;
                    if (!string.IsNullOrWhiteSpace(attachment.Path))
                    {
                        path = string.Join(Path.DirectorySeparatorChar, attachment.Path.Split(Path.DirectorySeparatorChar).SkipLast(1)) + Path.DirectorySeparatorChar + attachment.Name;
                    }
                    else
                    {
                        path = attachment.Name;
                    }
                    var entry = zipArchive.CreateEntry(path);
                    //using var fileStream = new FileStream(attachment.OriginalFolderPath, FileMode.Open);
                    //using var entryStream = entry.Open();
                    //await fileStream.CopyToAsync(entryStream);

                    byte[] newBytes;

                    using (var stream = new FileStream(attachment.OriginalFolderPath, FileMode.Open))
                    {
                        using (var entryStream = entry.Open())
                        {
                            byte[] bytes = new byte[stream.Length];
                            int numBytesToRead = (int)stream.Length;
                            int numBytesRead = 0;
                            while (numBytesToRead > 0)
                            {
                                // Read may return anything from 0 to numBytesToRead.
                                int n = stream.Read(bytes, numBytesRead, numBytesToRead);

                                // Break when the end of the file is reached.
                                if (n == 0)
                                    break;

                                numBytesRead += n;
                                numBytesToRead -= n;
                            }
                            newBytes = AesOperation.DecryptStream(bytes, _pathHelper.EncryptionKey);
                            entryStream.Write(newBytes);
                        }
                    }

                }
            }
            msFinal.Position = 0;
            return File(msFinal.ToArray(), "application/zip");
        }

        /// <summary>
        /// Folders the notification.
        /// </summary>
        /// <param name="notificationCommand">The notification command.</param>
        /// <returns></returns>
        [HttpPost("notification")]
        public async Task<IActionResult> FolderNotification(NotificationCommand notificationCommand)
        {
            var result = await _mediator.Send(notificationCommand);
            return Ok(result);
        }
    }
}
