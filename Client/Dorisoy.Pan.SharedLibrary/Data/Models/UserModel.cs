using System;
using System.Net;
using Macross.Json.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using MessagePack;

namespace Dorisoy.Pan.SharedLibrary.Data.Models;

[Serializable]
public class UserModel : IUnionModel
{
    //[Key(0)]
    public string UserId { get; set; }

    //[Key(1)]
    public string UserName { get; set; }

    //[Key(2)]
    public string Password { get; set; }

    //[Key(3)]
    public string Ip { get; set; }

    //[Key(4)]
    public int Port { get; set; }
}
