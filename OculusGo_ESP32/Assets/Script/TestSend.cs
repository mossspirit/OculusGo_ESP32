﻿using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class TestSend : MonoBehaviour {

    // broadcast address
    string host = "172.20.10.2";
    int port = 3333;
    private UdpClient client;

    void Start()
    {
        client = new UdpClient();
        client.Connect(host, port);
    }

    void Update()
    {
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 40), "Send"))
        {
            byte[] dgram = Encoding.UTF8.GetBytes("hello!");
            client.Send(dgram, dgram.Length);
        }
    }

    void OnApplicationQuit()
    {
        client.Close();
    }
}
