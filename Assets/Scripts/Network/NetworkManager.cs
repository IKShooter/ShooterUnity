using System;
using System.Collections.Generic;
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

    private NetworkPacketsBufferizer _networkPacketsBufferizer;

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

        _networkPacketsBufferizer = new NetworkPacketsBufferizer(this);
        
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
        netPP.RegisterNestedType(() => new RequestReload());
        netPP.RegisterNestedType(() => new RequestSwitchWeaponModel());
        netPP.RegisterNestedType(() => new RequestShootModel());
        netPP.RegisterNestedType(() => new RequestRespawn());
        
        //model
        netPP.RegisterNestedType(() => new RoomModel());
        netPP.RegisterNestedType(() => new PlayerModel());
        netPP.RegisterNestedType(() => new ErrorResultModel());
        netPP.RegisterNestedType(() => new RoomsListModel());
        netPP.RegisterNestedType(() => new SuccessAuthModel());
        netPP.RegisterNestedType(() => new JoinedRoomModel());
        netPP.RegisterNestedType(() => new PlayersInRoom());
        netPP.RegisterNestedType(() => new UpdatePlayerInRoom());
        netPP.RegisterNestedType(() => new UpdatePlayersTickInRoom());
        netPP.RegisterNestedType(() => new UpdateLocalPlayerInfo());
        netPP.RegisterNestedType(() => new RemotePlayerWeaponModel());
        netPP.RegisterNestedType(() => new LocalPlayerWeaponModel());
        netPP.RegisterNestedType(() => new ShootModel());
        netPP.RegisterNestedType(() => new LocalPlayerRespawnModel());
        netPP.RegisterNestedType(() => new DamageInfoModel());
        
        _netPacketProcessor.SubscribeNetSerializable((ErrorResultModel model, NetPeer peer) =>
        {
            EventsManager<ErrorEvent>.Trigger?.Invoke("Server error", new Exception($"{model.ErrorType.ToString()}"), model.IsCritical);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((ShootModel model, NetPeer peer) =>
        {
            EventsManager<PlayerShootEvent>.Trigger?.Invoke(model);
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
        
        _netPacketProcessor.SubscribeNetSerializable((UpdatePlayersTickInRoom model, NetPeer peer) =>
        {
            EventsManager<UpdatePlayerInRoomEvent>.Trigger?.Invoke(model);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((UpdateLocalPlayerInfo model, NetPeer peer) =>
        {
            EventsManager<LocalPlayerUpdateEvent>.Trigger?.Invoke(model);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((LocalPlayerRespawnModel model, NetPeer peer) =>
        {
            EventsManager<RespawnEvent>.Trigger?.Invoke(model.Pos, model.Rot);
        });
        
        _netPacketProcessor.SubscribeNetSerializable((DamageInfoModel model, NetPeer peer) =>
        {
            EventsManager<OtherDamageEvent>.Trigger?.Invoke(model);
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

    private float lastTickTime;
    private void Update()
    {
        if(_netManager.IsRunning)
        {
            _netManager.PollEvents();

            if (Time.time - lastTickTime > 1f / GameConstants.MaxTps)
            {
                Tick();
                lastTickTime = Time.time;
            }
        }
    }

    private void Tick()
    {
        _networkPacketsBufferizer.Tick();
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
        RequestPlayerAuthModel model = new RequestPlayerAuthModel
        {
            Nickname = nickName,
            VersionCode = GameConstants.VersionCode
        };
        PlanToSendModel(ref model);
    }

    public void SendModel<T>(ref T packet, bool isReliableOrdered = true) where T : Packet
    {
        _writer.Reset();
        _writer.Put(packet.Hash);
        packet.NetSerializable.Serialize(_writer);
        _serverPeer.Send(_writer, isReliableOrdered ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable);
    }

    public void PlanToSendModel<T>(ref T model, bool isHighPriority = false) where T : INetSerializable
    {
        _networkPacketsBufferizer.Add(model, isHighPriority);
    }

    public void LoadRoomsList()
    {
        RequestRoomsListModel model = new RequestRoomsListModel();
        PlanToSendModel(ref model);
    }

    public void TryCreateRoom(string name, int maxPlayers, string sceneName, string gameMode)
    {
        RequestCreateRoomAndJoinModel model = new RequestCreateRoomAndJoinModel()
        {
            NameRoom = name,
            GameMode = 0, // TODO
            MaxPlayers = (byte)maxPlayers,
            SceneName = sceneName
        };
        PlanToSendModel(ref model);
    }

    public void TryJoinRoom(string roomName)
    {
        RequestJoinRoomModel model = new RequestJoinRoomModel()
        {
            NameRoom = roomName,
        };
        PlanToSendModel(ref model);
    }

    public void LeaveRoom()
    {
        currentRoom = null;
        EventsManager<RoomLeaveEvent>.Trigger?.Invoke();
        
        RequestLeaveRoomModel model = new RequestLeaveRoomModel();
        PlanToSendModel(ref model);
    }

    public void RequestAccessInRoom()
    {
        RequestRoomAccessModel model = new RequestRoomAccessModel();
        PlanToSendModel(ref model);
    }

    public void TrySendMessage(string text, TypeMessage type)
    {
        RequestSendRoomMessageModel model = new RequestSendRoomMessageModel()
        {
            Text = text,
            Type = type
        };
        PlanToSendModel(ref model);
    }

    public void UpdatePlayer(Vector3 transformPosition, float verticalRotation, float horizontalRotation)
    {
        RequestUpdatePlayerInRoom model = new RequestUpdatePlayerInRoom()
        {
            Position = transformPosition,
            RotationY = verticalRotation,
            RotationCameraX = horizontalRotation
        };
        PlanToSendModel(ref model, isHighPriority: true);
    }

    public void TrySwitchWeapon(short slotId)
    {
        RequestSwitchWeaponModel model = new RequestSwitchWeaponModel()
        {
            SlotId = slotId
        };
        PlanToSendModel(ref model);
    }

    public void TryReloadWeapon()
    {
        RequestReload model = new RequestReload();
        PlanToSendModel(ref model);
    }

    public void TryShoot(Vector3 hitPos, bool isHit, int  targetPlayerId)
    {
        RequestShootModel model = new RequestShootModel()
        {
            IsHit = isHit,
            PosTo = hitPos,
            TargetPlayerId = targetPlayerId
        };
        PlanToSendModel(ref model);
    }

    public void Respawn()
    {
        RequestRespawn model = new RequestRespawn();
        PlanToSendModel(ref model);
    }
}