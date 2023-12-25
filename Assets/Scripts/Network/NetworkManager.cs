using System;
using System.Net;
using System.Net.Sockets;
using Events;
using LiteNetLib;
using LiteNetLib.Utils;
using Network;
using Network.Models;
using Server.Models;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    // private bool _isConnected;
    private NetManager _netManager;
    public NetPacketProcessor _netPacketProcessor;
    private NetDataWriter _writer;
    private NetPeer _serverPeer;

    private RoomModel currentRoom;
    
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

        NetPacketProcessor netPP = _netPacketProcessor;
        
        //request model
        netPP.RegisterNestedType(() => new RequestPlayerAuthModel());
        netPP.RegisterNestedType(() => new RequestRoomsListModel());
        netPP.RegisterNestedType(() => new RequestCreateRoomAndJoinModel());
        netPP.RegisterNestedType(() => new RequestRoomAccessModel());
        netPP.RegisterNestedType(() => new RequestJoinRoomModel());
        netPP.RegisterNestedType(() => new RequestSendRoomMessageModel());
        //model
        netPP.RegisterNestedType(() => new RoomModel());
        netPP.RegisterNestedType(() => new PlayerModel());
        netPP.RegisterNestedType(() => new ErrorResultModel());
        netPP.RegisterNestedType(() => new RoomsListModel());
        netPP.RegisterNestedType(() => new SuccessAuthModel());
        netPP.RegisterNestedType(() => new JoinedRoomModel());

        _netPacketProcessor.SubscribeNetSerializable((SuccessAuthModel model, NetPeer peer) =>
        {
            EventsManager<SuccessAuthEvent>.Trigger?.Invoke();
        });
        
        _netPacketProcessor.SubscribeNetSerializable((RoomsListModel model, NetPeer peer) =>
        {
            EventsManager<RoomsLoadedEvent>.Trigger?.Invoke(model.roomListModel);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((JoinedRoomModel model, NetPeer peer) =>
        {
            currentRoom = model.Room;
            EventsManager<RoomJoinedEvent>.Trigger?.Invoke(model.Room);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((MessageModel model, NetPeer peer) =>
        {
            EventsManager<MessageReceivedEvent>.Trigger?.Invoke(model);
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
        RequestPlayerAuthModel model = new RequestPlayerAuthModel
        {
            Nickname = nickName
        };
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }

    public void LoadRoomsList()
    {
        _writer.Reset();
        RequestRoomsListModel model = new RequestRoomsListModel();
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }

    public void TryCreateRoom(string name, int maxPlayers, string sceneName, string gameMode)
    {
        _writer.Reset();
        RequestCreateRoomAndJoinModel model = new RequestCreateRoomAndJoinModel()
        {
            NameRoom = name,
            GameMode = 0, // TODO
            MaxPlayers = (byte)maxPlayers,
            SceneName = sceneName
        };
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }

    public void TryJoinRoom(string roomName)
    {
        _writer.Reset();
        RequestJoinRoomModel model = new RequestJoinRoomModel()
        {
            NameRoom = roomName,
        };
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }

    public void LeaveRoom()
    {
        currentRoom = null;
        EventsManager<RoomLeaveEvent>.Trigger?.Invoke();
        
        // TODO: Send leave request
    }

    public void RequestAccessInRoom()
    {
        _writer.Reset();
        RequestRoomAccessModel model = new RequestRoomAccessModel();
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }

    public void TrySendMessage(string text, TypeMessage type)
    {
        _writer.Reset();
        RequestSendRoomMessageModel model = new RequestSendRoomMessageModel()
        {
            Text = text,
            Type = type
        };
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }
}