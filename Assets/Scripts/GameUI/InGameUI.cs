using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Text chatText;
    [SerializeField] private Text chatField;
    [SerializeField] private Dropdown messageTarget;
    
    void Start()
    {
        EventsManager<MessageReceivedEvent>.Register(message =>
        {
            string sender = message.Type == TypeMessage.System ? "SYSTEM" : message.SenderNickname;
            Debug.Log($"MESSAGE: {sender} : {message.Text}");   
            
            chatText.text += $"{sender} : {message.Text}\n";
        });
        
        NetworkManager.Instance.RequestAccessInRoom();
    }

    public void SendMessage()
    {
        byte typeId = Convert.ToByte(messageTarget.value);
        TypeMessage type = (TypeMessage)typeId;
        NetworkManager.Instance.TrySendMessage(chatField.text, type);
        chatField.text = "";
    }
}
