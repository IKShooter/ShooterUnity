using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Network;
using Server.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Text chatPreviewText;
    [SerializeField] private Text chatText;
    [SerializeField] private InputField chatField;
    [SerializeField] private Text chatFieldText;
    [SerializeField] private Dropdown messageTarget;

    [SerializeField] private GameObject chatUI;
    [SerializeField] private GameObject playersTableUI;

    private bool _isLeaved;
    private bool _isChatOpen;
    private bool _isPlayersFieldOpen;
    
    void Start()
    {
        EventsManager<MessageReceivedEvent>.Register(OnMessageReceived);
        NetworkManager.Instance.RequestAccessInRoom();
        
        // Hide by default
        playersTableUI.SetActive(false);
        chatUI.SetActive(false);
    }

    private void OnDestroy()
    {
        EventsManager<MessageReceivedEvent>.Unregister(OnMessageReceived);
    }

    private void Update()
    {
        // Players filed show/hide
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _isPlayersFieldOpen = true;
            playersTableUI.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _isPlayersFieldOpen = false;
            playersTableUI.SetActive(false);
        }
        
        // Chat show/hide toggle
        if (Input.GetKeyDown(KeyCode.U))
        {
            _isChatOpen = !_isChatOpen;
            chatUI.SetActive(_isChatOpen);
            
            Cursor.lockState = _isChatOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private void OnMessageReceived(MessageModel message)
    {
        if(_isLeaved)
            return;
        string sender = message.Type == TypeMessage.System ? "SYSTEM" : message.SenderNickname;
        Debug.Log($"MESSAGE: {sender} : {message.Text}");   
            
        chatText.text += $"{sender} : {message.Text}\n";

        if(!_isChatOpen)
            StartCoroutine(ShowPreviewChatMessage(message));
    }

    private IEnumerator ShowPreviewChatMessage(MessageModel message)
    {
        chatPreviewText.enabled = true;
        string sender = message.Type == TypeMessage.System ? "SYSTEM" : message.SenderNickname;
        chatPreviewText.text = $"{sender} : {message.Text}";
        yield return new WaitForSeconds(2.5f);
        chatPreviewText.enabled = false;
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
