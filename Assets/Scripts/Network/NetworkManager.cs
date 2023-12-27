using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Events;
using LiteNetLib;
using LiteNetLib.Utils;
using Network;
using Network.Models;
using Network.Models.Player;
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

    private PlayerModel currentPlayer;
    private RoomModel currentRoom;

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
        netPP.RegisterNestedType(() => new RequestLeaveRoomModel());
        netPP.RegisterNestedType(() => new RequestUpdatePlayerInRoom());
        
        //model
        netPP.RegisterNestedType(() => new RoomModel());
        netPP.RegisterNestedType(() => new PlayerModel());
        netPP.RegisterNestedType(() => new ErrorResultModel());
        netPP.RegisterNestedType(() => new RoomsListModel());
        netPP.RegisterNestedType(() => new SuccessAuthModel());
        netPP.RegisterNestedType(() => new JoinedRoomModel());
        netPP.RegisterNestedType(() => new PlayersInRoom());
        netPP.RegisterNestedType(() => new UpdatePlayerInRoom());

        _netPacketProcessor.SubscribeNetSerializable((ErrorResultModel model, NetPeer peer) =>
        {
            EventsManager<ErrorEvent>.Trigger?.Invoke("Server error", new Exception($"{model.ErrorType.ToString()}"), model.IsCritical);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((SuccessAuthModel model, NetPeer peer) =>
        {
            EventsManager<SuccessAuthEvent>.Trigger?.Invoke();
        });
        
        _netPacketProcessor.SubscribeNetSerializable((RoomsListModel model, NetPeer peer) =>
        {
            // BUG!?: Dublicates of rooms
            // FIXME: Stupid fuck

            List<RoomModel> filteredRooms = new List<RoomModel>();
            foreach (var roomModel in model.roomListModel)
            {
                bool isBugDublicate = false;
                foreach (var filteredRoom in filteredRooms)
                {
                    if (filteredRoom.Name == roomModel.Name)
                    {
                        Debug.Log($"SKIPPED BUG DUBLICATE {roomModel.Name}");
                        isBugDublicate = true;
                        break;
                    }
                }
                
                // Skip dublicate
                if(isBugDublicate)
                    continue;

                filteredRooms.Add(roomModel);
            }

            EventsManager<RoomsLoadedEvent>.Trigger?.Invoke(filteredRooms);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((JoinedRoomModel model, NetPeer peer) =>
        {
            currentRoom = model.Room;
            currentPlayer = model.PlayerModel;
            EventsManager<RoomJoinedEvent>.Trigger?.Invoke(model.Room);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((MessageModel model, NetPeer peer) =>
        {
            EventsManager<MessageReceivedEvent>.Trigger?.Invoke(model);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((PlayersInRoom model, NetPeer peer) =>
        {
            EventsManager<PlayersInRoomEvent>.Trigger?.Invoke(model.Players);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((UpdatePlayerInRoom model, NetPeer peer) =>
        {
            EventsManager<UpdatePlayerInRoomEvent>.Trigger?.Invoke(model);
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

    public PlayerModel GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public RoomModel GetCurrentRoom()
    {
        return currentRoom;
    }

    public void TryConnect()
    {
        _netManager.Start();
        _netManager.Connect(GameConstants.ServerAddr, GameConstants.ServerPort, GameConstants.ServerKey);
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
            Nickname = nickName,
            VersionCode = GameConstants.VersionCode
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
        
        _writer.Reset();
        RequestLeaveRoomModel model = new RequestLeaveRoomModel();
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
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

    public void UpdatePlayer(Vector3 transformPosition, float verticalRotation)
    {
        _writer.Reset();
        RequestUpdatePlayerInRoom model = new RequestUpdatePlayerInRoom()
        {
            Position = transformPosition,
            RotationY = verticalRotation
        };
        _netPacketProcessor.WriteNetSerializable(_writer, ref model);
        _serverPeer.Send(_writer, DeliveryMethod.Unreliable);
    }
}