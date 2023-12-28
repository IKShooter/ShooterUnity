using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using GameUI;
using Network;
using Network.Models;
using Network.Models.Player;
using Player;
using Server.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Text chatText;
    [SerializeField] private InputField chatField;
    [SerializeField] private Text chatFieldText;
    [SerializeField] private Dropdown messageTarget;

    [SerializeField] private Text statText;

    [SerializeField] private GameObject chatUI;
    [SerializeField] private PlayersListInRoomUI playersTableUI;

    private bool _isLeaved;
    private bool _isChatOpen;
    private bool _isPlayersFieldOpen;

    private float lastMessageTime = 0f;
    
    private List<MessageModel> allMessages = new List<MessageModel>();
    
    void Start()
    {
        EventsManager<MessageReceivedEvent>.Register(OnMessageReceived);
        EventsManager<PlayersInRoomEvent>.Register(OnPlayersInRoomUpdated);
        EventsManager<LocalPlayerUpdateEvent>.Register(OnLocalPlayerUpdated);
        NetworkManager.Instance.RequestAccessInRoom();
        
        // Hide by default
        playersTableUI.gameObject.SetActive(false);
        chatUI.SetActive(false);
    }

    private void OnDestroy()
    {
        EventsManager<LocalPlayerUpdateEvent>.Unregister(OnLocalPlayerUpdated);
        EventsManager<MessageReceivedEvent>.Unregister(OnMessageReceived);
        EventsManager<PlayersInRoomEvent>.Unregister(OnPlayersInRoomUpdated);
    }

    private void Update()
    {
        // Players filed show/hide
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _isPlayersFieldOpen = true;
            playersTableUI.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _isPlayersFieldOpen = false;
            playersTableUI.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        // Chat show/hide toggle
        if (Input.GetKeyDown(KeyCode.U))
        {
            _isChatOpen = !_isChatOpen;
            chatUI.SetActive(_isChatOpen);

            if (_isChatOpen)
            {
                ShowAllChatHistory();
            }
            else
            {
                chatText.text = "";
            }
            
            Cursor.lockState = _isChatOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (lastMessageTime != 0f && Time.time - lastMessageTime > 2.5f)
        {
            chatText.text = "";
        }
    }

    private void UpdateStat(UpdateLocalPlayerInfo model)
    {
        statText.text = "";
        statText.text += $"HP: {model.Health}\n\n";
        statText.text += $"{model.CurrentWeapon.Id}\n";
        statText.text += $"{model.CurrentWeapon.Ammo} / {model.CurrentWeapon.AmmoReserve}";
    }

    public void OnLocalPlayerUpdated(UpdateLocalPlayerInfo model)
    {
        PlayerController.Instance.PlayerWeaponComponent.UpdateByModel(model);
        UpdateStat(model);
    }

    private void OnPlayersInRoomUpdated(List<PlayerModel> players)
    {
        playersTableUI.UpdateList(players);
    }
    
    private void OnMessageReceived(MessageModel message)
    {
        if(_isLeaved) return;
        
        allMessages.Add(message);
        
        string sender = message.Type == TypeMessage.System ? "SYSTEM" : message.SenderNickname;
        chatText.text += $"{sender} : {message.Text}\n";

        lastMessageTime = Time.time;
    }

    public void ShowAllChatHistory()
    {
        chatText.text = "";
        foreach (var msg in allMessages)
        {
            string sender = msg.Type == TypeMessage.System ? "SYSTEM" : msg.SenderNickname;
            chatText.text += $"{sender} : {msg.Text}\n";
        }
    }
    
    public void SendMessage()
    {
        byte typeId = Convert.ToByte(messageTarget.value);
        TypeMessage type = (TypeMessage)typeId;
        NetworkManager.Instance.TrySendMessage(chatFieldText.text, type);
        
        // FIXME: 2 different ways to clean fcking input field
        chatFieldText.text = "";
        chatField.Select();
        chatField.SetTextWithoutNotify("");
    }

    public void LeaveFromRoom()
    {
        _isLeaved = true;
        NetworkManager.Instance.LeaveRoom();
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
