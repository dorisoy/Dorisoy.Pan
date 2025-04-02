using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration.UserSecrets;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Dorisoy.Pan.Helper
{
    public class ThumbnailHelper
    {
        private static List<string> audioFileExtension = new List<string> {
                ".3gp",".aa",".aac",".aax",".act",".aiff",".alac",".amr",".ape",".au",".awb",".dss",".dvf",".flac",
                ".gsm",".iklx",".ivs",".m4a",".m4b",".m4p",".mmf",".mp3",".mpc",".msv",".nmf",".ogg",".oga",".mogg",
                ".opus",".org",".ra",".rm",".raw",".rf64",".sln",".tta",".voc",".vox",".wav",".wma",".wv",".webm",
            };
        private static List<string> videoFileExtension = new List<string>
        {
            ".webm",".flv",".vob",".ogv",".ogg",".drc",".avi",".mts",".m2ts",".wmv",".yuv",".viv",".mp4",".m4p",
            ".3pg",".flv",".f4v",".f4a"
        };

        private static List<string> compressedFileExtension = new List<string>
        {
            ".gzip",".zip"
        };
        public static string GetThumbnailFile(string documentPath, string path)
        {
            if (path.IndexOf(zoomName) != -1)
            {
                return Path.Combine(documentPath, path);
            }
            return path;
        }
        static string zoomName = "_thumbnail_";
        public static string SaveThumbnailFile(IFormFile file, string name, string filePath,string documentPath,string key)
        {
            try
            {
                // Images
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (file.ContentType.StartsWith("image/"))
                {
                    try
                    {
                        var bytes = File.ReadAllBytes(Path.Combine(filePath, name));
                        using var image = Image.Load(AesOperation.DecryptStream(bytes, key));
                        image.Mutate(x => x.Resize(96, 96));
                        var thumPath = Path.Combine(documentPath, "Thumbnails");
                        if (!Directory.Exists(thumPath))
                        {
                            Directory.CreateDirectory(thumPath);
                        }
                        var path = Path.Combine(thumPath, zoomName + name);
                        image.Save(path);
                        return Path.Combine("Thumbnails", $"{zoomName}{name}");
                    }
                    catch
                    {
                        return Path.Combine("Thumbnails", "image.png");
                    }
                }
                else if (fileExtension == ".doc" || fileExtension == ".docx")
                {
                    return Path.Combine("Thumbnails", "word.png");
                }
                else if (fileExtension == ".pdf")
                {
                    return Path.Combine("Thumbnails", "pdf.png");
                }
                else if (fileExtension == ".pptx" || fileExtension == ".ppt")
                {
                    return Path.Combine("Thumbnails", "ppt.png");
                }
                else if (fileExtension == ".csv")
                {
                    return Path.Combine("Thumbnails", "csv.png");
                }
                else if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    return Path.Combine("Thumbnails", "excel.png");
                }
                else if (fileExtension == ".txt")
                {
                    return Path.Combine("Thumbnails", "text.png");
                }
                else if (fileExtension == ".json")
                {
                    return Path.Combine("Thumbnails", "json.png");
                }
                else if (fileExtension == ".accdb")
                {
                    return Path.Combine("Thumbnails", "ms_db.png");
                }
                else if (fileExtension == ".sql")
                {
                    return Path.Combine("Thumbnails", "sql.png");
                }
                else if (fileExtension == ".rar")
                {
                    return Path.Combine("Thumbnails", "rar.png");
                }
                else if (fileExtension == ".7z")
                {
                    return Path.Combine("Thumbnails", "7z.png");
                }
                else if (videoFileExtension.IndexOf(fileExtension) >= 0)
                {
                    return Path.Combine("Thumbnails", "video.png");
                }
                else if (audioFileExtension.IndexOf(fileExtension) >= 0)
                {
                    return Path.Combine("Thumbnails", "audio.png");
                }
                else if (compressedFileExtension.IndexOf(fileExtension) >= 0)
                {
                    return Path.Combine("Thumbnails", "zip.png");
                }
                else
                {
                    return Path.Combine("Thumbnails", "unknow_file.png");
                }
            }
            catch
            {

                return Path.Combine("Thumbnails", "unknow_file.png");
            }
        }

        public static bool IsSystemThumnails(string path)
        {
            return path.StartsWith("Thumbnails");
        }
    }
}
