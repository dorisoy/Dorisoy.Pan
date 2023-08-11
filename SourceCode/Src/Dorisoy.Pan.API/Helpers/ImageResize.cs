using Dorisoy.Pan.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using ImageInfo = Dorisoy.Pan.Data.ImageInfo;

namespace Dorisoy.Pan.API.Helpers
{
    public static class ImageResize
    {

        public static void resizeImage(ImageInfo imageInfo, string pathToSave)
        {
            string imageData = imageInfo.Src.Split(',').LastOrDefault();
            byte[] bytes = Convert.FromBase64String(imageData);
            using (MemoryStream ms = new MemoryStream(bytes))
            {

                using (Image image = Image.Load(ms))
                {
                    // Resize the image in place and return it for chaining.
                    // 'x' signifies the current image processing context.
                    image.Mutate(x => x.Resize(imageInfo.Width, imageInfo.Height));

                    // The library automatically picks an encoder based on the file extension then
                    // encodes and write the data to disk.
                    // You can optionally set the encoder to choose.
                    image.Save(pathToSave);
                }
            }
        }

        public static void customImageWithOutResize(string imageSoruce, string pathToSave)
        {
            string imageData = imageSoruce.Split(',').LastOrDefault();
            byte[] bytes = Convert.FromBase64String(imageData);
            using (MemoryStream ms = new MemoryStream(bytes))
            {

                using (Image image = Image.Load(ms))
                {
                    // Resize the image in place and return it for chaining.
                    // 'x' signifies the current image processing context.
                    image.Mutate(x => x.Resize(200, 200));
                    // The library automatically picks an encoder based on the file extension then
                    // encodes and write the data to disk.
                    // You can optionally set the encoder to choose.
                    image.Save(pathToSave);
                }
            }
        }
        public static void precriptionImageWithOutResize(string imageSoruce, string pathToSave)
        {
            string imageData = imageSoruce.Split(',').LastOrDefault();
            byte[] bytes = Convert.FromBase64String(imageData);
            using (MemoryStream ms = new MemoryStream(bytes))
            {

                using (Image image = Image.Load(ms))
                {
                    // Resize the image in place and return it for chaining.
                    // 'x' signifies the current image processing context.
                    //image.Mutate(x => x.Resize(200, 200));
                    // The library automatically picks an encoder based on the file extension then
                    // encodes and write the data to disk.
                    // You can optionally set the encoder to choose.
                    image.Save(pathToSave);
                }
            }
        }
    }
}
