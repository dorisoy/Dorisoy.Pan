using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorisoy.Pan.SharedLibrary.Data.Models;
using System.Text.Json.Serialization;
using MessagePack;

namespace Dorisoy.Pan.SharedLibrary.Data.UDPData;

[MessagePackObject]
public class NewAudioFrame
{
    [Key(0)]
    public AudioFrameModel audioFrame { get; set; }

    [SerializationConstructor]
    public NewAudioFrame(byte[] newFrame, string userId, Dictionary<Guid, string> keys)
    {
        audioFrame = new AudioFrameModel
        {
            Users = keys,
            UserId = userId,
            FrameBytes = newFrame
        };
    }
}
