using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketClient
{
    private SocketClient sInstance;

    Thread receiveThread;
    UdpClient client;
    int port = 5065;

    Action<String> onReceiveFunc;

    SocketClient()
    {
        InitThread();
    }

    SocketClient(int port)
    {
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
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
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

    public void OnReceive(Action<String> func)
    {
        onReceiveFunc = func;
    }

    public void Send(String data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        client.Send(bytes, bytes.Length);
    }


}