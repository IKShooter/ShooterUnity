using System;
using System.Net;
using System.Net.Sockets;
using Events;
using LiteNetLib;
using LiteNetLib.Utils;
using Network;
using Network.Models;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    // private bool _isConnected;
    private NetManager _netManager;
    public NetPacketProcessor _netPacketProcessor;
    private NetDataWriter _writer;
    private NetPeer _serverPeer;
    
    private const string Key = "Shooter";
    private const string Addr = "10.0.0.5";
    private const int Port = 7332;

    public NetworkManager()
    {
        Instance = this;
    }
    
    private void Start()
    {
        DontDestroyOnLoad(this);

        _netManager = new NetManager(new NetworkEventsController(this));
        _netPacketProcessor = new NetPacketProcessor();
        _writer = new NetDataWriter();
        
        _netPacketProcessor.RegisterNestedType<RequestPlayerAuthModel>(() => new RequestPlayerAuthModel());

        _netPacketProcessor.SubscribeNetSerializable((SuccessAuthModel model, NetPeer peer) =>
        {
            EventsManager<SuccessAuthEvent>.Trigger?.Invoke();
        });
        
        EventsManager<ServerConnectedEvent>.Register((peer) =>
        {
            _serverPeer = peer;
            Debug.Log("Connected!");
        });
        
        EventsManager<ServerDisconnectedEvent>.Register((peer, reason) =>
        {
            _serverPeer = peer;
            Debug.Log("Disconnected!");
        });
        
        TryConnect();
    }
    
    private void Update()
    {
        if(_netManager.IsRunning)
        {
            _netManager.PollEvents();
        }
    }

    private void OnApplicationQuit()
    {
        TryDisconnected();
    }

    public void TryConnect()
    {
        _netManager.Start();
        _netManager.Connect(Addr, Port, Key);
    }

    public void TryDisconnected()
    {
        _netManager.Stop();
    }

    public void TryAuth(string nickName)
    {
        _writer.Reset();
        RequestPlayerAuthModel requestPlayerAuthModel = new RequestPlayerAuthModel
        {
            Nickname = nickName
        };
        _netPacketProcessor.WriteNetSerializable(_writer, ref requestPlayerAuthModel);
        _serverPeer.Send(_writer, DeliveryMethod.Unreliable);
    }
}