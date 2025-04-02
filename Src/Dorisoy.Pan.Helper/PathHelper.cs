using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Dorisoy.Pan.Helper
{
    public class PathHelper
    {
        public IConfiguration _configuration;
        public string _contentRootPath;
        public PathHelper(
            IConfiguration configuration,
            string contentPath
            )
        {
            this._configuration = configuration;
            _contentRootPath = contentPath;
        }
        public string DocumentPath
        {
            get
            {
                return _configuration["DocumentPath"];
            }
        }
        public string EncryptionKey
        {
            get
            {
                return _configuration["EncryptionKey"];
            }
        }

        public string UserProfilePath
        {
            get
            {
                return _configuration["UserProfilePath"];
            }
        }

        public List<string> ExecutableFileTypes
        {
            get
            {
                try
                {
                    return _configuration["ExecutableFileTypes"].Split(',').ToList();
                }
                catch
                {
                    return new List<string>();
                }

            }
        }

        public string DefaultUserImage
        {
            get
            {
                return _configuration["DefaultUserImage"];
            }
        }

        public string[] WebApplicationUrl
        {
            get
            {
                return string.IsNullOrEmpty(_configuration["WebApplicationUrl"]) ? new string[] { } : _configuration["WebApplicationUrl"].Split(",");
            }
        }
        public string ContentRootPath
        {
            get
            {
                var contentRootPath = _configuration["ContentRootPath"];
                if (string.IsNullOrWhiteSpace(contentRootPath))
                {
#if DEBUG
                    _contentRootPath = "wwwroot";
#endif
                    return _contentRootPath;
                }
                else
                {
                    return contentRootPath;
                }

            }
        }

    }
}
