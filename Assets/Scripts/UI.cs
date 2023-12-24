using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using LiteNetLib;
using LiteNetLib.Utils;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(param);
        });
    }
}

public class UI : MonoBehaviour
{
    private static UI mInstance = null;

    public static UI Instance => mInstance;

    static NetManager Client => ClientConnect.client;

    public GameObject ImageScene;

    public Dropdown MaxPlayer;
    public Dropdown SceneName;
    public Dropdown GM;

    public InputField NameRoom;


    public GameObject panelAuth;
    public GameObject panelCreateRoom;
    public GameObject panelServerList;

    public GameObject templateButtonRoom => Resources.Load<GameObject>("Prefabs/Button");
    public GameObject reserveButtonRoom;
    public GameObject parentContentViewListRoom;
    public GameObject buttonServerList;
    public GameObject buttonCreatorRoom;

    void Start()
    {
        SceneName.onValueChanged.AddListener(DropDownOnValueChangedSceneName);
    }
    
    public void DropDownOnValueChangedSceneName(int index)
    {
        switch(index)
        {
            case 0:
                ImageScene.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Scenes/Zombi");
                break;
            case 1:
                ImageScene.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Scenes/Angar");
                break;
        }
    }
    

    public void LeaveToMainMenu()
    {
        ActionMessage actionMessage = new ActionMessage
        {
            ActionCode = ActionCode.PlayerLeave,
            Data = "LeaveToMainMenu"
        };

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);

        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);

        SceneManager.LoadScene("MainMenu");
    }

    public void GetRoomList(List<Room> rooms)
    {
        DeleteListRoom();
        if (rooms == null) return;
        for (int i = 0; i < rooms.Count; i++)
        {
            reserveButtonRoom = Instantiate(templateButtonRoom, parentContentViewListRoom.transform);
            
            reserveButtonRoom.GetComponent<Button>().AddEventListener(i, ItemClicked);

            reserveButtonRoom.transform.GetChild(0).GetComponent<Text>().text = rooms[i].SceneName;
            reserveButtonRoom.transform.GetChild(1).GetComponent<Text>().text = rooms[i].NameRoom;
            reserveButtonRoom.transform.GetChild(2).GetComponent<Text>().text = $"{rooms[i].CurrentPlayers} / {rooms[i].MaxPlayers}";
            reserveButtonRoom.transform.GetChild(3).GetComponent<Text>().text = rooms[i].GameMode.ToString();

            Debug.Log(rooms[i].NameRoom);
        }
    }

    public void RefreshRooms()
    {
        string text = "GetServerList";

        ActionMessage actionMessage = new ActionMessage
        {
            ActionCode = ActionCode.GetServerList,
            Data = text
        };

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);
        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);

        GetRoomList(ClientListener.Rooms);
    }

    void ItemClicked(int ItemIndex)
    {
        JoinRoom(parentContentViewListRoom.transform.GetChild(ItemIndex).GetChild(1).GetComponent<Text>().text);
    }

    private void JoinRoom(string NameRoom)
    {
        ActionMessage actionMessage = new ActionMessage
        {
            ActionCode = ActionCode.JoinRoom,
            Data = NameRoom
        };

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);
        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    void Update()
    {
        if(mInstance == null) mInstance = this;

        buttonCreatorRoom.GetComponent<Button>().interactable = ClientListener.IsConnect;
        buttonServerList.GetComponent<Button>().interactable = ClientListener.IsConnect;
    }

    public void DeleteListRoom()
    {
        int countChildButton = parentContentViewListRoom.transform.childCount;
        for (int i = 0; i < countChildButton; i++)
        {
            GameObject transform = parentContentViewListRoom.transform.GetChild(i).gameObject;
            if (transform.GetComponent<Button>()) Destroy(transform);
        }
    }

    public struct ActionMessage
    {
        public ActionCode ActionCode { get; set; }
        public string Data { get; set; }
    }

    public void CreateRoom()
    {
        string maxPlayerText = MaxPlayer.options[MaxPlayer.value].text;
        string sceneName = SceneName.options[SceneName.value].text;
        GameMode gameMode = GameMode.DM;

        try
        {
            if (Enum.TryParse(GM.options[GM.value].text, out gameMode))
            {

            }
        }
        catch (Exception e)
        {
            Debug.LogError($"CreateRoom Error: {e.Message} ");
        }

        Room room = new Room() {
            SceneName = sceneName,
            NameRoom = NameRoom.text,
            MaxPlayers = int.Parse(maxPlayerText),
            GameMode = gameMode
        };

        string data = JsonConvert.SerializeObject(room);

        ActionMessage actionMessage = new ActionMessage() {
            ActionCode = ActionCode.CreateRoom,
            Data = data
        };

        NetDataWriter writer = new NetDataWriter();

        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);

        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }
}

class Auth
{
    public string codeAuth;
    public string nickname;
}