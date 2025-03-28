using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;
using Dorisoy.Pan.Helper;
using System.Text;
using Dorisoy.Pan.Common;
using System.Web;
using Azure;
using System.Linq;

namespace Dorisoy.Pan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly PathHelper _pathHelper;

        public DocumentController(IMediator mediator,
         PathHelper pathHelper)
        {
            _mediator = mediator;
            _pathHelper = pathHelper;
        }


        /// <summary>
        /// Gets the document token.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="isVersion"></param>
        /// <returns></returns>
        [HttpGet("{id}/token")]
        [AllowAnonymous]
        public async Task<ActionResult> GetDocumentToken(Guid id, bool isVersion)
        {
            var getDocumentTokenQuery = new GetDocumentTokenQuery
            {
                Id = id,
                IsVersion = isVersion
            };
            var result = await _mediator.Send(getDocumentTokenQuery);
            return Ok(new { result });
        }

        /// <summary>
        /// Deletes the document token.
        /// </summary>
        /// <param name="token">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("token/{token}")]
        public async Task<ActionResult> DeleteDocumentToken(Guid token)
        {
            var deleteDocumentTokenCommand = new DeleteDocumentTokenCommand { Token = token };
            var result = await _mediator.Send(deleteDocumentTokenCommand);
            return Ok(result);
        }

        /// <summary>
        /// Gets the document file by token.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="token">The identifier.</param>
        /// <param name="isVersion"></param>
        /// <returns></returns>
        [HttpGet("{id}/officeviewer")]
        [AllowAnonymous]
        public async Task<ActionResult> GetDocumentFileByToken(Guid id, Guid token, bool isVersion)
        {
            var deleteDocumentTokenCommand = new GetDocumentPathByTokenCommand
            {
                Id = id,
                Token = token,
                IsVersion = isVersion
            };
            var result = await _mediator.Send(deleteDocumentTokenCommand);
            if (!result.Success)
            {
                return NotFound();
            }
            string filePath = result.Data;
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            byte[] newBytes;
            using (var stream = new FileStream(filePath, FileMode.Open))
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
            }

            //var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(newBytes, contentType, Path.GetFileName(filePath));
        }

        /// <summary>
        /// Move Document
        /// </summary>
        /// <param name="moveDocumentCommand"></param>
        /// <returns></returns>
        [HttpPost("move")]
        public async Task<IActionResult> MoveDocument(MoveDocumentCommand moveDocumentCommand)
        {
            var result = await _mediator.Send(moveDocumentCommand);
            return Ok(result);
        }
        /// <summary>
        /// Copy Document
        /// </summary>
        /// <param name="copyDocumentCommand"></param>
        /// <returns></returns>

        [HttpPost("copy")]
        public async Task<IActionResult> CopyDocument(CopyDocumentCommand copyDocumentCommand)
        {
            var result = await _mediator.Send(copyDocumentCommand);
            return Ok(result);
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("folder/{id}")]
        public async Task<IActionResult> GetDocumentByFolderId(Guid id)
        {
            var query = new GetAllDocumentsQuery { FolderId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var query = new DeleteDocumentCommand { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Document Viewer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/viewer")]
        public async Task<IActionResult> DocumentViewer(Guid id)
        {
            var commnad = new GetDocumentViewerQuery { DocumentId = id };
            var result = await _mediator.Send(commnad);
            return Ok(result);
        }

        /// <summary>
        /// Downloads the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="isVersion"></param>
        /// <returns></returns>
        [HttpGet("{id}/download")]
        //[AllowAnonymous]
        public async Task<IActionResult> DownloadDocument(Guid id, bool isVersion)
        {
            var commnad = new DownloadDocumentCommand
            {
                Id = id,
                IsVersion = isVersion
            };
            var path = await _mediator.Send(commnad);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), path);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");
            var fileName = HttpUtility.UrlEncode(Path.GetFileName(filePath));
            Response.ContentType = "application/octet-stream";
            Response.Headers.Append(new System.Collections.Generic.KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>("Content-Disposition", "attachment; filename=" + fileName));
            await LargeFileEncryptor.DecryptFile(filePath, _pathHelper.EncryptionKey, async (buff) =>
            {
                if(Request.HttpContext.RequestAborted.IsCancellationRequested)
                {
                    return false;
                }
                await Response.Body.WriteAsync(buff);
                await Response.Body.FlushAsync();
                return true;
            });
            return new EmptyResult();
        }

        /// <summary>
        /// Download document using token.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <param name="isVersion"></param>
        /// <returns></returns>
        [HttpGet("{id}/download/token/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadDocument(Guid id, string token, bool isVersion)
        {
            var commnad = new DownloadDocumentCommand
            {
                Id = id,
                IsVersion = isVersion,
                IsFromPreview = true,
                Token = token
            };
            var path = await _mediator.Send(commnad);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), path);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            byte[] newBytes;
            using (var stream = new FileStream(filePath, FileMode.Open))
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
            }

            return File(newBytes, GetContentType(filePath), filePath);
        }

        /// <summary>
        /// Read text Document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isVersion"></param>
        /// <returns></returns>
        [HttpGet("{id}/readText")]
        public async Task<IActionResult> ReadTextDocument(Guid id, bool isVersion)
        {
            var commnad = new DownloadDocumentCommand
            {
                Id = id,
                IsVersion = isVersion
            };
            var path = await _mediator.Send(commnad);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), path);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            byte[] newBytes;
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
                var latestBytes = memory.ToArray();
                newBytes = AesOperation.DecryptStream(latestBytes, _pathHelper.EncryptionKey);
            }
            string utfString = Encoding.UTF8.GetString(newBytes, 0, newBytes.Length);
            // var result = System.IO.File.ReadAllLines(filePath);
            return Ok(new { result = new string[] { utfString } });
        }

        /// <summary>
        /// Read Text using token.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <param name="isVersion"></param>
        /// <returns></returns>
        [HttpGet("{id}/readText/token/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ReadTextDocument(Guid id, string token, bool isVersion)
        {
            var commnad = new DownloadDocumentCommand
            {
                Id = id,
                IsVersion = isVersion,
                IsFromPreview = true,
                Token = token
            };
            var path = await _mediator.Send(commnad);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), path);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");


            byte[] newBytes;
            using (var memory = new MemoryStream())
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                    var latestBytes = memory.ToArray();
                    newBytes = AesOperation.DecryptStream(latestBytes, _pathHelper.EncryptionKey);
                }
            }
            string utfString = Encoding.UTF8.GetString(newBytes, 0, newBytes.Length);
            // var result = System.IO.File.ReadAllLines(filePath);
            return Ok(new { result = new string[] { utfString } });


            //var result = System.IO.File.ReadAllLines(filePath);
            //return Ok(new { result });
        }

        /// <summary>
        /// Removes the document right for user.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpDelete("{documentId}/remove-right/user/{userId}")]
        public async Task<IActionResult> RemoveDocumentRightForUser(Guid documentId, Guid userId)
        {
            var removeFolderRightForUserCommand = new RemoveDocumentRightForUserCommand
            {
                DocumentId = documentId,
                UserId = userId,
            };
            var result = await _mediator.Send(removeFolderRightForUserCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Searches the document.
        /// </summary>
        /// <param name="searchString">The search string.</param>
        /// <returns></returns>
        [HttpGet("search/{searchString}")]
        public async Task<IActionResult> SearchDocument(string searchString)
        {
            var commnad = new SearchDocumentQuery { SearchString = searchString };
            var documents = await _mediator.Send(commnad);
            return Ok(documents);
        }

        /// <summary>
        /// Renames the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="renameDocumentCommand">The rename document command.</param>
        /// <returns></returns>
        [HttpPut("{id}/rename")]
        public async Task<IActionResult> RenameDocument(Guid id, RenameDocumentCommand renameDocumentCommand)
        {
            renameDocumentCommand.Id = id;
            var result = await _mediator.Send(renameDocumentCommand);
            return ReturnFormattedResponse(result);
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
