using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Text chatText;
    [SerializeField] private Text chatField;
    [SerializeField] private Dropdown messageTarget;

    [SerializeField] private GameObject chatUI;
    [SerializeField] private GameObject playersTableUI;

    private bool _isLeaved;
    private bool _isChatOpen = true;
    
    void Start()
    {
        EventsManager<MessageReceivedEvent>.Register(message =>
        {
            if(_isLeaved)
                return;
            string sender = message.Type == TypeMessage.System ? "SYSTEM" : message.SenderNickname;
            Debug.Log($"MESSAGE: {sender} : {message.Text}");   
            
            chatText.text += $"{sender} : {message.Text}\n";
        });
        
        NetworkManager.Instance.RequestAccessInRoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            _isChatOpen = !_isChatOpen;
            chatUI.SetActive(_isChatOpen);
            
            Cursor.lockState = _isChatOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void SendMessage()
    {
        byte typeId = Convert.ToByte(messageTarget.value);
        TypeMessage type = (TypeMessage)typeId;
        NetworkManager.Instance.TrySendMessage(chatField.text, type);
        chatField.text = "";
    }

    public void LeaveFromRoom()
    {
        _isLeaved = true;
        NetworkManager.Instance.LeaveRoom();
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
