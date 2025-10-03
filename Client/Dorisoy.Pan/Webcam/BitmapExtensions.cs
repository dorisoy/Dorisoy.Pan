using System.Drawing;
using System.Drawing.Imaging;
using Avalonia.Media.Imaging;
using Path = System.IO.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace Dorisoy.PanClient.Webcam;

[SupportedOSPlatform("windows")]
public static class BitmapExtensions
{

    public static System.Drawing.Bitmap ToSystemBitmap(this Avalonia.Media.Imaging.Bitmap bitmap)
    {
        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream);
        return (new System.Drawing.Bitmap(memoryStream));
    }

    public static Avalonia.Media.Imaging.Bitmap ConvertToAvaloniaImage(this System.Drawing.Bitmap image)
    {
        using var stream = new MemoryStream();
        image.Save(stream, ImageFormat.Jpeg);
        image.Dispose();
        stream.Seek(0, SeekOrigin.Begin);
        var avaloniaImage = new Avalonia.Media.Imaging.Bitmap(stream);
        return avaloniaImage;


        //using var ms = new MemoryStream(bytes);
        //return new Avalonia.Media.Imaging.Bitmap(ms);
    }

    public static Avalonia.Media.Imaging.Bitmap ConvertToAvaloniaBitmap(this System.Drawing.Bitmap bitmap, bool dispose = true)
    {
        return ConvertToAvaloniaBitmap(bitmap, dispose, 0, 0);
    }

    public static Avalonia.Media.Imaging.Bitmap ConvertToAvaloniaBitmap(this System.Drawing.Bitmap bitmap, bool dispose = true, int newW = 0, int newH = 0)
    {
        if (bitmap == null)
            return null;

        try
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            if (newW > 0 && newH > 0)
                rect = new Rectangle(0, 0, newW, newH);

            var bitmapdata = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            //RGB8888:分别用8个bit来记录每个像素的A、R、G、B数据，就是常说的32bit位图、256色位图(这个也可能是RGB888这种24bit位图)
            var newBitmap = new Avalonia.Media.Imaging.Bitmap(
                Avalonia.Platform.PixelFormat.Bgra8888,
                AlphaFormat.Premul,
                bitmapdata.Scan0,
                new PixelSize(bitmapdata.Width, bitmapdata.Height),
                new Vector(96, 96),
                bitmapdata.Stride);

            bitmap.UnlockBits(bitmapdata);
            bitmap.Dispose();

            return newBitmap;

        }
        catch (Exception)
        {
            return null;
        }
    }

    public static byte[] BitmapToBytes(this Avalonia.Media.Imaging.Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream);
        byte[] bytes = stream.ToArray();
        return bytes;
    }


    public static Avalonia.Media.Imaging.Bitmap ConvertToWpfImage(this System.Drawing.Bitmap bitmap)
    {
        if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

        var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

        var bitmapData = bitmap.LockBits(
            rect,
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppArgb);

        try
        {
            //var size = (rect.Width * rect.Height) * 4;

            var newBitmap = new Avalonia.Media.Imaging.Bitmap(Avalonia.Platform.PixelFormat.Bgra8888,
                AlphaFormat.Premul,
                bitmapData.Scan0,
                new PixelSize(bitmapData.Width, bitmapData.Height),
                new Vector(96, 96),
                bitmapData.Stride);

            //BitmapInterpolationMode.HighQuality

            return newBitmap;
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();
        }
    }

    public static Avalonia.Media.Imaging.Bitmap ToAvaloniaBitmap( this WriteableBitmap writeableBitmap)
    {
        //var tempFile = Path.GetTempFileName();
        //bitmap.Save(tempFile);
        //var result = new Avalonia.Media.Imaging.Bitmap(tempFile);
        //File.Delete(tempFile);
        //return (result);

        if (writeableBitmap != null)
        {
            // 将 WriteableBitmap 保存到内存流中
            using var memoryStream = new MemoryStream();
            writeableBitmap.Save(memoryStream);
            memoryStream.Position = 0;
            var avaloniaImage = new Avalonia.Media.Imaging.Bitmap(memoryStream);
            return avaloniaImage;
        }
        else
        {
            return null;
        }
    }


    public static Avalonia.Media.Imaging.Bitmap GetBitmapFromRGBBuffer(byte[] buffer, int width, int height)
    {
        System.Drawing.Bitmap b = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

        System.Drawing.Rectangle BoundsRect = new Rectangle();
        BoundsRect.Width = width;
        BoundsRect.Height = height;

        BitmapData bmpData = b.LockBits(BoundsRect, ImageLockMode.WriteOnly, b.PixelFormat);

        IntPtr ptr = bmpData.Scan0;

        // add back dummy bytes between lines, make each line be a multiple of 4 bytes
        int skipByte = bmpData.Stride - width * 3;
        byte[] newBuff = new byte[buffer.Length + skipByte * height];
        for (int j = 0; j < height; j++)
        {
            Buffer.BlockCopy(buffer, j * width * 3, newBuff, j * (width * 3 + skipByte), width * 3);
        }

        // fill in rgbValues
        Marshal.Copy(newBuff, 0, ptr, newBuff.Length);
        b.UnlockBits(bmpData);
        return ConvertToAvaloniaBitmap(b, true);
    }


    /// <summary>
    /// 您可以使用 WriteableBitmap 和 WriteableBitmap.Lock API 而不是 System.Drawing.Bitma 
    /// WriteableBitmap API 使用原始指针进行操作，您可以向其中写入数据。
    /// </summary>
    /// <param name="rgbPixelData"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static WriteableBitmap CreateBitmapFromPixelData(byte[] rgbPixelData, int width, int height)
    {
        Vector dpi = new Vector(96, 96);
        var bitmap = new WriteableBitmap(new PixelSize(width, height), dpi, Avalonia.Platform.PixelFormat.Rgba8888);

        using (var frameBuffer = bitmap.Lock())
        {
            Marshal.Copy(rgbPixelData, 0, frameBuffer.Address, rgbPixelData.Length);
        }
        return bitmap;
    }

    public static Avalonia.Media.Imaging.Bitmap ToBitmap(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        return new Avalonia.Media.Imaging.Bitmap(ms);
    }

    public static System.Drawing.Bitmap SaveCompressedBitmap(System.Drawing.Bitmap originalBitmap, long quality)
    {
        // 定义JPEG编码器
        ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);

        // 创建一个EncoderParameters对象
        EncoderParameters encoderParameters = new EncoderParameters(1);

        // 设置图片的压缩质量
        EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
        encoderParameters.Param[0] = encoderParameter;

        // 使用内存流来保存压缩后的数据
        using (var memoryStream = new MemoryStream())
        {
            originalBitmap.Save(memoryStream, jpegEncoder, encoderParameters);
            // 从内存流创建新的Bitmap
            memoryStream.Position = 0;// 重置内存流的位置
            return new System.Drawing.Bitmap(memoryStream);
        }
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }

        return null;
    }
}
