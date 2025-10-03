using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MessagePack;

namespace Dorisoy.Pan.SharedLibrary.Data.Models;

[Serializable]
public class AudioFrameModel : IUnionModel
{
    //[Key(0)]
    public string UserId { get; set; }

    //[Key(1)]
    public byte[] Bytes { get; set; }
}
