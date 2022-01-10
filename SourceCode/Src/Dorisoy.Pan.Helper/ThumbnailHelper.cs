using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;

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
            ".gzip",".7z",".zip"
        };
        public static string SaveThumbnailFile(IFormFile file, string name, string documentPath, string userId)
        {
            try
            {
                // Images
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (file.ContentType.StartsWith("image/"))
                {
                    try
                    {
                        using var image = Image.Load(file.OpenReadStream());
                        image.Mutate(x => x.Resize(100, 100));
                        if (!Directory.Exists($"{documentPath}"))
                        {
                            Directory.CreateDirectory($"{documentPath}");
                        }
                        var path = Path.Combine(documentPath, "_thumbnail_" + name);
                        image.Save(path);
                        return Path.Combine(userId, $"_thumbnail_{name}");
                    }
                    catch (Exception e)
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
