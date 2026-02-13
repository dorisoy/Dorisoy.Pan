using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class UploadDocumentCommand : IRequest<ServiceResponse<DocumentDto>>
    {
        public Guid FolderId { get; set; }
        public IFormFileCollection Documents { get; set; }
        public string FullPath { get; set; }
        /// <summary>
        /// 当前分片
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 总分片
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 文件md5
        /// </summary>
        public string Md5 { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }
    }
}
