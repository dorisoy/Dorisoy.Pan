using H264Sharp;
using Mat = OpenCvSharp.Mat;

namespace Dorisoy.PanClient.Services.Video;


/// <summary>
/// 用于表示一个图像引用
/// </summary>
public class ImageReference
{
    /// <summary>
    /// 基础数据
    /// </summary>
    public object underlyingData;

    public bool isManaged;
    private IntPtr dataStart;

    public byte[] Data;
    public int Offset;
    public int Length;

    /// <summary>
    /// PixelSize（宽度和高度），Stride行跨度
    /// </summary>
    public int Width, Height, Stride;


    public Action ReturnImage;

    /// <summary>
    /// 指向图像像素缓冲区的指针
    /// </summary>
    public IntPtr DataStart {
        get
        {

            if (!isManaged)
                return dataStart;
            else
            {
                unsafe
                {
                    fixed (byte* p = &Data[Offset])
                    {
                        return  (IntPtr)(p);
                    }
                }

            }
        }
        set => dataStart = value; }

    public ImageReference(object underlyingData, IntPtr dataPtr, int width, int height, int stride, Action<ImageReference> whenReturning)
    {
        Create(underlyingData, dataPtr, width, height, stride, () => whenReturning.Invoke(this));
    }
    private void Create(object underlyingData, IntPtr dataPtr, int width, int height, int stride, Action whenReturning)
    {
        this.underlyingData = underlyingData;
        this.DataStart = dataPtr;
        Width = width;
        Height = height;
        Stride = stride;
        ReturnImage = whenReturning;
    }

    public ImageReference( byte[] data, int offset, int length, int width, int height, int stride)
    {
        Data = data;
        Offset = offset;
        Length = length;
        Width = width;
        Height = height;
        Stride = stride;
        isManaged = true;
    }

    public void Update(Mat mat)
    {
        
         Create(mat, mat.DataStart, mat.Width, mat.Height, (int)mat.Step(), ReturnImage);
    }
    internal void Update(RgbImage mat)
    {
        //Create(mat, mat.ImageBytes, mat.Width, mat.Height, mat.Stride, ReturnImage);
    }
    public static ImageReference FromMat(Mat mat,Action<ImageReference> whenReturning)
    {
        return new ImageReference(mat, mat.DataStart, mat.Width, mat.Height,(int)mat.Step(), whenReturning);
    }

   

    public void Release()
    {
        if(underlyingData!=null && underlyingData is IDisposable)
            ((IDisposable)underlyingData).Dispose();
    }

    
}
