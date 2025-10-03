using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Bitmap = System.Drawing.Bitmap;
using Encoder = System.Drawing.Imaging.Encoder;
using Rectangle = System.Drawing.Rectangle;

namespace Dorisoy.PanClient.Utils;

public class CompressImage
{
    public Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
    {
        try
        {
            Bitmap bitmap = new Bitmap(newW, newH);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            graphics.Dispose();
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    public void Compress_Image(ref MemoryStream ms)
    {
        Bitmap bitmap = new Bitmap(ms);
        ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
        Encoder quality = Encoder.Quality;
        EncoderParameters encoderParameters = new EncoderParameters(1);
        EncoderParameter encoderParameter = new EncoderParameter(quality, 25L);
        encoderParameters.Param[0] = encoderParameter;
        try
        {
            bitmap.Save(ms, encoderInfo, encoderParameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void Compress_Image(string imageFile, string saveName)
    {
        Bitmap bitmap = new Bitmap(imageFile);
        ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
        Encoder quality = System.Drawing.Imaging.Encoder.Quality;
        EncoderParameters encoderParameters = new EncoderParameters(1);
        EncoderParameter encoderParameter = new EncoderParameter(quality, 25L);
        encoderParameters.Param[0] = encoderParameter;
        try
        {
            bitmap.Save(saveName, encoderInfo, encoderParameters);
        }
        catch
        {
        }
    }

    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
        for (int i = 0; i < imageEncoders.Length; i++)
        {
            if (imageEncoders[i].MimeType == mimeType)
            {
                return imageEncoders[i];
            }
        }

        return null;
    }
}
