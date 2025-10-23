using System.Drawing.Imaging;


namespace Dorisoy.Pan;

public static class PixelBufferExt
{
    /// <summary>
    /// 将IntPtr（指向图像像素缓冲区的指针）复制到可写入位图
    /// </summary>
    /// <param name="writeableBitmap"></param>
    /// <param name="sourcePtr"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="stride"></param>
    public static void CopyIntPtrToWriteableBitmap(WriteableBitmap writeableBitmap, IntPtr sourcePtr, int width, int height, int stride)
    {
        // 锁定 WriteableBitmap，准备写入数据
        using (var lockedBuffer = writeableBitmap.Lock())
        {
            // 遍历图片的每一行
            for (int y = 0; y < height; y++)
            {
                // 创建一个字节数组来存储这一行的数据
                byte[] rowData = new byte[width * 3];// 假设是32位位图，每个像素4字节

                // 源指针偏移，定位到当前行的起始位置
                IntPtr srcPtrOffset = IntPtr.Add(sourcePtr, y * stride);

                // 从非托管内存复制到托管数组
                Marshal.Copy(srcPtrOffset, rowData, 0, rowData.Length);

                // 将托管数组的内容复制到WriteableBitmap
                for (int i = 0; i < rowData.Length; i++)
                {
                    // 计算目标位置指针
                    IntPtr destPtrOffset = IntPtr.Add(lockedBuffer.Address, y * lockedBuffer.RowBytes + i);
                    // 写入数据
                    Marshal.WriteByte(destPtrOffset, rowData[i]);
                }
            }
        }
    }

    public static byte[] GetRowBytes(this ILockedFramebuffer buffer, int y)
    {
        // Create an array to hold the row bytes
        var rowLength = buffer.Size.Width * ((int)buffer.Format.BitsPerPixel / 8);
        var rowBytes = new byte[rowLength];

        // Calculate the start pointer of the row
        IntPtr startPtr = buffer.Address + y * buffer.RowBytes;

        // Copy the row bytes into the array
        Marshal.Copy(startPtr, rowBytes, 0, rowLength);

        return rowBytes;
    }


    public static WriteableBitmap CreateBitmapFromPixelData(byte[] bgraPixelData, int pixelWidth, int pixelHeight)
    {
        var dpi = new Vector(96, 96);
        var bitmap = new WriteableBitmap(new PixelSize(pixelWidth, pixelHeight), dpi, Avalonia.Platform.PixelFormat.Bgra8888);
        using (var frameBuffer = bitmap.Lock())
        {
            Marshal.Copy(bgraPixelData, 0, frameBuffer.Address, bgraPixelData.Length);
        }
        return bitmap;
    }


    // buffer size = width*height*3
    public static Avalonia.Media.Imaging.Bitmap GetBitmapFromRGBBuffer(byte[] buffer, int width, int height)
    {
        System.Drawing.Bitmap b = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        System.Drawing.Rectangle BoundsRect = new System.Drawing.Rectangle();
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
        return ConvertToAvaloniaBitmap(b);
    }

    public static Avalonia.Media.Imaging.Bitmap ConvertToAvaloniaBitmap(System.Drawing.Bitmap bitmap)
    {
        if (bitmap == null)
            return null;
        System.Drawing.Bitmap bitmapTmp = bitmap;
        var bitmapdata = bitmapTmp.LockBits(new System.Drawing.Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888,
            AlphaFormat.Premul, bitmapdata.Scan0,
            new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
            new Avalonia.Vector(96, 96),
            bitmapdata.Stride);
        bitmapTmp.UnlockBits(bitmapdata);
        bitmapTmp.Dispose();
        return bitmap1;
    }



}
