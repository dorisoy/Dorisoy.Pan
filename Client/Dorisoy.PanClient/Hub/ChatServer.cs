using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinol.CaptureManager.SharedLibrary.SerDes;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using DataObject = Sinol.CaptureManager.SharedLibrary.Data.DataObject;

namespace Sinol.CaptureManager.Hub;

public static class ChatServer
{
    private const int _serverTcpPort = 9933;
    private static int _serverUdpPort;
    private static string _serverHost = "192.168.0.163";


    public static void setUdpPort(int serverUdpPort)
    {
        _serverUdpPort = serverUdpPort;
        //_serverHost = ip;
        remoteUdpIpEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverUdpPort);
    }


    public static int getUdpPort()
    {
        return _serverUdpPort;
    }
    public static void clearUdpPort()
    {
        _serverUdpPort = -1;
        remoteUdpIpEndPoint = null;
    }
    private static UdpClient _udpClient;
    private static IPEndPoint remoteTcpIpEndPoint;
    private static IPEndPoint remoteUdpIpEndPoint;
    private static Socket _serverTcpSocket;


    static ChatServer()
    {
        _serverUdpPort = -1;
        _udpClient = new UdpClient();

        remoteTcpIpEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverTcpPort);

        remoteUdpIpEndPoint = null;
        _serverTcpSocket = null;
    }


    public static void connectTcp()
    {
        try
        {
            if (_serverTcpSocket == null || !_serverTcpSocket.Connected)
            {
                _serverTcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverTcpSocket.ReceiveTimeout = 1000 * 10;
                _serverTcpSocket.Connect(remoteTcpIpEndPoint);
            }
        }
        catch
        {
            //App.MessageBox("无法连接到服务器,（TCP）");
            throw;
        }
    }
    public static void sendTcp(DataObject dataObject)
    {
        try
        {
            _serverTcpSocket.Send(Serializer.serialize(dataObject));
        }
        catch
        {
            //App.MessageBox("TCP数据在服务器上时出错");
            throw;
        }
    }

    public static void sendUdp(DataObject dataObject)
    {
        if (_serverUdpPort == -1)
            return;
        try
        {
            var bytes = Serializer.serialize(dataObject);
            _udpClient.SendAsync(bytes, bytes.Length, remoteUdpIpEndPoint);
        }
        catch
        {
            //App.MessageBox("分析从UDP到服务器的数据时出错");
            throw;
        }
    }

    public static DataObject listenToServerTcpResponse()
    {
        try
        {
            while (_serverTcpSocket.Connected)
            {
                byte[] buffer = new byte[8000];
                try
                {
                    _serverTcpSocket.Receive(buffer);
                }
                catch { }
                return SerializeHelper.Deserialize(buffer);
            }

            return null;
        }
        catch
        {
            //App.MessageBox("TCP套接字侦听失败");
            throw;
        }
    }
}
