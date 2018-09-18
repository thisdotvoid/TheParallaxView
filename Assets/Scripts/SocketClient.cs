using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient
{
    private static SocketClient sInstance;

    Thread receiveThread;
    UdpClient client;
    string server = "127.0.0.1";
    int port = 5065;

    Action<string> onReceiveFunc;

    SocketClient()
    {
        InitThread();
    }

    SocketClient(string server, int port)
    {
        this.server = server;
        this.port = port;
        InitThread();
    }

    public static SocketClient Get() {
        if (sInstance == null) {
            sInstance = new SocketClient();
        }

        return sInstance;
    }

    private void InitThread()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse(server), port);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                if (onReceiveFunc != null)
                {
                    onReceiveFunc(text);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    public void OnReceive(Action<string> func)
    {
        onReceiveFunc = func;
    }

    public void Send(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        client.Send(bytes, bytes.Length);
    }


}