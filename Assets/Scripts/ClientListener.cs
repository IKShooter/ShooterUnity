using LiteNetLib;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Runtime.InteropServices;
using LiteNetLib.Utils;
using UnityEngine.SceneManagement;

public class ClientListener : INetEventListener
{
    static NetManager Client => ClientConnect.client;

    public static Room CurrenRoom;

    public static bool IsConnect = false;

    public struct ActionMessage
    {
        public ActionCode ActionCode { get; set; }
        public string Data { get; set; }
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        int actionCodeInt = reader.GetInt();
        ActionCode actionCode = (ActionCode)actionCodeInt;

        string receivedData = reader.GetString();

        HandleActionByCode(peer, actionCode, receivedData);
    }

    private void HandleActionByCode(NetPeer peer, ActionCode actionCode, string data)
    {
        switch (actionCode)
        {
            case ActionCode.OperationSuccess:
                Debug.Log($"OperationSuccess: {data}");
                HandleOperationSuccess(data);
                break;
            case ActionCode.Error:
                Debug.Log($"OperationError: {data}");
                break;
            case ActionCode.Auth:
                Auth(data);
                break;
            case ActionCode.GetServerList:
                GetServerList(data);
                break;
            case ActionCode.NewMessage:
                ChatManager.Instance.NewMessage(data);
                break;
            case ActionCode.PlayerLeave:
                PlayerLeave(data);
                break;
            case ActionCode.JoinRoom: 
                JoinRoom(data);
                break;
            case ActionCode.GetClassRoom:
                GetClassRoom(data);
                break;
            case ActionCode.JoinPlayerRoom:
                JoinPlayerRoom(data);
                break;
            case ActionCode.UpdateInfoRoom:
                UpdateRoomInfo(data);
                break;
            case ActionCode.SpawnPlayer:
                Player player = JsonConvert.DeserializeObject<Player>(data);
                RoomLogic.Instance.SpawnPlayer(player);
                break;
            case ActionCode.SpawnLocalPlayer:
                RoomLogic.Instance.SpawnLocalPlayer(data);
                break;
            case ActionCode.UpdatePosition:
                UpdatePositions(data);
                break;
        }
    }

    private void HandleOperationSuccess(string data)
    {
        switch(data)
        {
            case "JoinRoom":
                RequestClassRoom();
                break;
            case "Auth":
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    public void Auth(string data)
    {
        Player player = JsonConvert.DeserializeObject<Player>(data);
        ClientConnect.Instance.LocalPlayer = player;
    }

    public static void SendData(string data, ActionCode actionCode)
    {
        NetDataWriter writer = new NetDataWriter();

        ActionMessage actionMessage = new ActionMessage
        {
            ActionCode = actionCode,
            Data = data
        };

        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);

        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void UpdatePositions(string data)
    {
        Player player = JsonConvert.DeserializeObject<Player>(data);
        RoomLogic.Instance.MovePlayer(player);
        Debug.Log($"Player Pos: x {player.Position.x} | y {player.Position.y} | z {player.Position.z}");
    }

    private void UpdateRoomInfo(string data)
    {
        CurrenRoom = JsonConvert.DeserializeObject<Room>(data);
        BattleUI.Instance.SpawnPlayersInTabList();
        ChatManager.Instance.FillDropDownSelectPlayer();
        Debug.Log("Update room info");
    }

    public void JoinPlayerRoom(string data)
    {
        if(ClientConnect.Instance.stateScene == ClientConnect.StateScene.InRoom)
        {
            Player player = JsonConvert.DeserializeObject<Player>(data);
        }
    }

    public void RequestClassRoom()
    {
        ActionMessage actionMessage = new ActionMessage()
        {
            ActionCode = ActionCode.GetClassRoom,
            Data = ""
        };

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);
        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void GetClassRoom(string data)
    {
        try
        {
            Room room = JsonConvert.DeserializeObject<Room>(data);

            CurrenRoom = room;

            SceneManager.LoadSceneAsync(room.SceneName);

            ChatManager.Instance.FillDropDownSelectPlayer();

            Debug.Log($"Join to: {room.NameRoom} | scene: {room.SceneName} | players: {room.Players.Count}");
        }
        catch(Exception e)
        {
            Debug.LogError("GetClassRoom: " + e.Message);
        }
    }

    private void JoinRoom(string data)
    {
        Player player = JsonConvert.DeserializeObject<Player>(data);
        Debug.Log($"Player join: {player.NickName}");

        BattleUI.Instance.SpawnPlayersInTabList();
    }

    public void PlayerLeave(string data)
    {
        Player player = JsonConvert.DeserializeObject<Player>(data);
        RoomLogic.Instance.DestroyPlayer(player);

        Debug.Log($"Player {player.NickName} Leave");

        BattleUI.Instance.SpawnPlayersInTabList();
    }

    public static List<Room> Rooms = new List<Room>();

    public void GetServerList(string data)
    {
        Debug.Log("GetServerList");
        List<Room> list = JsonConvert.DeserializeObject<List<Room>>(data);
        Rooms = list;
        UI.Instance.GetRoomList(list);
    }
    #region other
    public void OnConnectionRequest(ConnectionRequest request)
    {
    }
    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.Log($"OnNetworkError: {socketError}");
    }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }
    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("OnPeerConnected");
        IsConnect = true;
    }
    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log($"OnPeerDisconnected: {disconnectInfo.Reason}");
        IsConnect = false;
    }
    #endregion
}

public enum PermissionLevel : byte
{
    Owner,
    Administrator,
    Moderator,
    ChatModerator,
    None
}

public class Permissions : INetSerializable
{
    public PermissionLevel PermissionLevel;
    public bool IsMuted;
    public bool IsBanned;

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {

    }
}

public enum GameMode
{
    DM,
    TDM,
    ZM
}

public class Room : INetSerializable
{
    public string SceneName;
    public string NameRoom;
    public List<Player> Players = new List<Player>();
    public GameMode GameMode;
    public int CurrentPlayers;
    public int MaxPlayers;
    public bool IsPrivateRoom;
    public string Password;

    public void Deserialize(NetDataReader reader)
    {
        SceneName = reader.GetString();
        NameRoom = reader.GetString();


    }

    public void Serialize(NetDataWriter writer)
    {

    }
}

public enum PlayerSceneState : byte
{
    InMainMenu,
    InRoom
}