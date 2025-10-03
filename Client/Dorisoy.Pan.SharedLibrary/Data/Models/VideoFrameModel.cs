using System;

namespace Dorisoy.Pan.SharedLibrary.Data.Models;

public enum DataObjectTypes
{
    Login,
    LoginOut,
    UserList
}

[Serializable]
public class UserModel 
{
    public string UserId { get; set; }
    public DataObjectTypes DataTypes { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
}



[Serializable]
public class AudioFrameModel 
{
    public string UserId { get; set; }
    public byte[] Bytes { get; set; }
}
