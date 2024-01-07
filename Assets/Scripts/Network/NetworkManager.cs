using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.Models;
using Network.Models.Player;
using Server.Models;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance;
        public static NetworkManager Instance => _instance;
        
        private NetworkPacketsBufferizer _networkPacketsBufferizer;

        public NetPacketProcessor NetPacketProcessor;
        private NetManager _netManager;
        private NetDataWriter _writer;
        private NetPeer _serverPeer;

        private PlayerModel _currentPlayer;
        private RoomModel _currentRoom;
        
        private float _lastTickTime;

        public NetworkManager()
        {
            if(_instance != null) {
                Debug.Log("UNSAFE! Network manager re-initializated");
            }
            _instance = this;
        }

        private Queue<Vector3> _debugPoints = new Queue<Vector3>();

        public void SetServerIp(string addr) {
            PlayerPrefs.SetString("server.ip", addr);
            PlayerPrefs.Save();
        } 

        public string GetServerIp() {
            return PlayerPrefs.GetString("server.ip", GetAvaliableServerIps()[0]);
        }

        public List<string> GetAvaliableServerIps() {
            return new List<string>() { "10.0.0.4", "10.0.0.5", "77.246.100.110", "127.0.0.1" };
        }

        public void ReConnect() {
            TryDisconnect();
            TryConnect();
        }

        public int GetPacketsInBufferCount()
        {
            return _networkPacketsBufferizer.GetPacketsCount();
        }
        
        public List<Vector3> GetDebugPoints()
        {
            float scale = 3f;
            _debugPoints.Enqueue(new Vector3((_debugPoints.Count - 1) * scale, GetPacketsInBufferCount() * scale));
            if (_debugPoints.Count >= 32)
                _debugPoints.Dequeue();

            return _debugPoints.ToList();
        } 
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            _netManager = new NetManager(new NetworkEventsController(this));
            // _netManager.ReconnectDelay = 1_000;
            // _netManager.MaxConnectAttempts = 2;
            NetPacketProcessor = new NetPacketProcessor();
            _writer = new NetDataWriter();

            _networkPacketsBufferizer = new NetworkPacketsBufferizer(this);
        
            NetPacketProcessor netPp = NetPacketProcessor;
        
            //request model
            netPp.RegisterNestedType(() => new RequestPlayerAuthModel());
            netPp.RegisterNestedType(() => new RequestRoomsListModel());
            netPp.RegisterNestedType(() => new RequestCreateRoomAndJoinModel());
            netPp.RegisterNestedType(() => new RequestRoomAccessModel());
            netPp.RegisterNestedType(() => new RequestJoinRoomModel());
            netPp.RegisterNestedType(() => new RequestSendRoomMessageModel());
            netPp.RegisterNestedType(() => new RequestLeaveRoomModel());
            netPp.RegisterNestedType(() => new RequestUpdatePlayerInRoom());
            netPp.RegisterNestedType(() => new RequestReload());
            netPp.RegisterNestedType(() => new RequestSwitchWeaponModel());
            netPp.RegisterNestedType(() => new RequestShootModel());
            netPp.RegisterNestedType(() => new RequestRespawn());
        
            //model
            netPp.RegisterNestedType(() => new RoomModel());
            netPp.RegisterNestedType(() => new PlayerModel());
            netPp.RegisterNestedType(() => new ErrorResultModel());
            netPp.RegisterNestedType(() => new RoomsListModel());
            netPp.RegisterNestedType(() => new SuccessAuthModel());
            netPp.RegisterNestedType(() => new JoinedRoomModel());
            netPp.RegisterNestedType(() => new PlayersInRoom());
            netPp.RegisterNestedType(() => new UpdatePlayerInRoom());
            netPp.RegisterNestedType(() => new UpdatePlayersTickInRoom());
            netPp.RegisterNestedType(() => new UpdateLocalPlayerInfo());
            netPp.RegisterNestedType(() => new RemotePlayerWeaponModel());
            netPp.RegisterNestedType(() => new LocalPlayerWeaponModel());
            netPp.RegisterNestedType(() => new ShootModel());
            netPp.RegisterNestedType(() => new LocalPlayerRespawnModel());
            netPp.RegisterNestedType(() => new DamageInfoModel());
        
            NetPacketProcessor.SubscribeNetSerializable((ErrorResultModel model, NetPeer peer) =>
            {
                EventsManager<ErrorEvent>.Trigger?.Invoke("Server error", new Exception($"{model.ErrorType.ToString()}"), model.IsCritical);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((ShootModel model, NetPeer peer) =>
            {
                EventsManager<PlayerShootEvent>.Trigger?.Invoke(model);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((SuccessAuthModel model, NetPeer peer) =>
            {
                EventsManager<SuccessAuthEvent>.Trigger?.Invoke();
            });
        
            NetPacketProcessor.SubscribeNetSerializable((RoomsListModel model, NetPeer peer) =>
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
        
            NetPacketProcessor.SubscribeNetSerializable((JoinedRoomModel model, NetPeer peer) =>
            {
                _currentRoom = model.Room;
                _currentPlayer = model.PlayerModel;
                EventsManager<RoomJoinedEvent>.Trigger?.Invoke(model.Room);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((MessageModel model, NetPeer peer) =>
            {
                EventsManager<MessageReceivedEvent>.Trigger?.Invoke(model);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((PlayersInRoom model, NetPeer peer) =>
            {
                EventsManager<PlayersInRoomEvent>.Trigger?.Invoke(model.Players);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((UpdatePlayersTickInRoom model, NetPeer peer) =>
            {
                EventsManager<UpdatePlayerInRoomEvent>.Trigger?.Invoke(model);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((UpdateLocalPlayerInfo model, NetPeer peer) =>
            {
                EventsManager<LocalPlayerUpdateEvent>.Trigger?.Invoke(model);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((LocalPlayerRespawnModel model, NetPeer peer) =>
            {
                EventsManager<RespawnEvent>.Trigger?.Invoke(model.Pos, model.Rot);
            });
        
            NetPacketProcessor.SubscribeNetSerializable((DamageInfoModel model, NetPeer peer) =>
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
                _serverPeer = null;
                Debug.Log($"Disconnected! Reason: {reason.Reason}");
            });
        
        
            TryConnect();
        }
        
        private void Update()
        {
            if(_netManager.IsRunning)
            {
                _netManager.PollEvents();

                if (Time.time - _lastTickTime > 1f / GameConstants.MaxTps)
                {
                    Tick();
                    _lastTickTime = Time.time;
                }
            }
        }

        private void Tick()
        {
            _networkPacketsBufferizer.Tick();
        }

        private void OnApplicationQuit()
        {
            TryDisconnect();
        }

        public PlayerModel GetCurrentPlayer()
        {
            return _currentPlayer;
        }

        public RoomModel GetCurrentRoom()
        {
            return _currentRoom;
        }

        public void TryConnect()
        {
            _netManager.Start();
            _netManager.Connect(GetServerIp(), GameConstants.ServerPort, GameConstants.ServerKey);
        }

        public void TryDisconnect()
        {
            //_serverPeer?.Disconnect();
            _netManager.Stop();
            EventsManager<ServerDisconnectedEvent>.Trigger?.Invoke(_serverPeer, new DisconnectInfo());
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

        public void TryCreateRoom(string roomName, int maxPlayers, string sceneName, string gameMode)
        {
            RequestCreateRoomAndJoinModel model = new RequestCreateRoomAndJoinModel()
            {
                NameRoom = roomName,
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
            _currentRoom = null;
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
}