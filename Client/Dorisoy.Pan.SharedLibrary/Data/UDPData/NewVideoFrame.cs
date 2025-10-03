using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MessagePack;
using Dorisoy.Pan.SharedLibrary.Data.Models;

namespace Dorisoy.Pan.SharedLibrary.Data.UDPData;

[MessagePackObject]
public class NewVideoFrame
{
    [Key(0)]
    public VideoFrameModel videoFrame { get; set; }

    //[SerializationConstructor]
    //public NewVideoFrame(Bitmap newFrame, string userId, Dictionary<Guid, string> keys)
    //{
    //    videoFrame = new VideoFrameModel();
    //    videoFrame.UserId = userId;
    //    videoFrame.Users = keys;

    //    try
    //    {
    //        using (var ms = new MemoryStream())
    //        {
    //            newFrame.Save(ms, ImageFormat.Jpeg);
    //            videoFrame.FrameBytes = ms.ToArray();
    //        }
    //    }
    //    catch
    //    {
    //        videoFrame.FrameBytes = null;
    //    }
    //}

    //[SerializationConstructor]
    //public NewVideoFrame(byte[] newFrame, string userId, Dictionary<Guid, string> keys)
    //{
    //    videoFrame = new VideoFrameModel
    //    {
    //        UserId = userId,
    //        Users = keys,
    //        FrameBytes = newFrame
    //    };
    //}
}
