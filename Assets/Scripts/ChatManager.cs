using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    private static ChatManager instance;
    public static ChatManager Instance => instance;

    static NetManager Client => ClientConnect.client;

    public InputField Message;

    public GameObject PanelChat;
    public GameObject ContentViewMessages;
    public GameObject G_InputFieldMessage;
    private GameObject reserveMessage;

    public Dropdown DropdownSelectorPlayer;

    public static int MaxMessages = 15;

    void Start()
    {        
        Message.characterLimit = 25;
    }

    public static bool IsChatOpen = false;

    void Update()
    {
        if(instance == null) instance = this;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!Message.gameObject.activeSelf)
            {
                Message.gameObject.SetActive(true);
                Message.ActivateInputField();
                Message.Select();
                Message.text = string.Empty;
            }
            else
            {
                if (!string.IsNullOrEmpty(Message.text))
                {
                    SendMsg(Message.text);
                    Message.DeactivateInputField();
                    Message.text = string.Empty;
                }
            }
        }
    }

    public void FillDropDownSelectPlayer()
    {
        DropdownSelectorPlayer.options.Clear();
        DropdownSelectorPlayer.options.Add(new Dropdown.OptionData() { text = "[Public]" });

        List<Player> players = ClientListener.CurrenRoom.Players;
        
        if(players.Count > 1)
        {
            foreach (Player player in players)
            {
                if (ClientConnect.Instance.LocalPlayer.NickName != player.NickName)
                {
                    DropdownSelectorPlayer.options.Add(new Dropdown.OptionData() { text = player.NickName });
                }
            }
        }
    }

    public struct ActionMessage
    {
        public ActionCode ActionCode { get; set; }
        public string Data { get; set; }
    }

    void SendMsg(string text)
    {
        Message message = new Message
        {
            SenderNickName = string.Empty,
            MessageText = text
        };

        string prefix = DropdownSelectorPlayer.options[DropdownSelectorPlayer.value].text;
        string original = text;

        string result;

        if (prefix != "[Public]")
        {
            result = $"{prefix}: " + original;
            message.MessageText = result;
        }

        string sendData  = JsonConvert.SerializeObject(message);

        ActionMessage actionMessage = new ActionMessage
        {
            ActionCode = ActionCode.SendMessage,
            Data = sendData
        };

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);
        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void NewMessage(string data)
    {
        Message message = JsonConvert.DeserializeObject<Message>(data);
        AddMessgeToContent(message);
        Debug.Log($"Player {message.SenderNickName} send message: {message.MessageText}");
    }

    public void AddMessgeToContent(Message msg)
    {
        GameObject templateMessage = Resources.Load<GameObject>("Prefabs/TextMessage");
        reserveMessage = Instantiate(templateMessage, ContentViewMessages.transform);
        reserveMessage.transform.GetComponent<Text>().text = $"{msg.SenderNickName}: {msg.MessageText}";

        if (ContentViewMessages.transform.childCount > MaxMessages)
        {
            Destroy(ContentViewMessages.transform.GetChild(0).gameObject);
        }
    }
}