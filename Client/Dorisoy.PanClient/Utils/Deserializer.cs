using System.Runtime.Serialization.Formatters.Binary;

namespace Sinol.CaptureManager.Utils;

///// <summary>
///// 序列化助手
///// </summary>
//public class SerializeHelper
//{

//    /**/
//    ///*****************************************
//    /// <summary>
//    /// 序列化一个对象
//    /// </summary>
//    /// <param name="o">将要序列化的对象</param>
//    /// <returns>返回byte[]</returns>
//    ///*****************************************
//    public static byte[] Serialize(object obj)
//    {
//        if (obj == null) return null;
//        BinaryFormatter formatter = new BinaryFormatter();
//        MemoryStream ms = new MemoryStream();

//#pragma warning disable SYSLIB0011
//        formatter.Serialize(ms, obj);
//#pragma warning restore SYSLIB0011

//        ms.Position = 0;
//        byte[] b = new byte[ms.Length];
//        ms.Read(b, 0, b.Length);
//        ms.Close();
//        return b;
//    }

//    /**/
//    ///*****************************************
//    /// <summary>
//    /// 反序列化
//    /// </summary>
//    /// <param name="b">返回一个对象</param>
//    ///*****************************************
//    public static object Deserialize(byte[] data)
//    {
//        if (data.Length == 0) return null;
//        try
//        {
//            BinaryFormatter bf = new BinaryFormatter();
//            MemoryStream ms = new MemoryStream();
//            ms.Write(data, 0, data.Length);
//            ms.Position = 0;

//#pragma warning disable SYSLIB0011
//            object n = (object)bf.Deserialize(ms);
//#pragma warning restore SYSLIB0011
//            ms.Close();
//            return n;
//        }
//        catch (Exception e)
//        {
//            //System.Runtime.Serialization.SerializationException:“Unable to find assembly 'SharedLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'.”
//            System.Diagnostics.Debug.WriteLine(e.ToString());
//            return null;
//        }
//    }

//}
