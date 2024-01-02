using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using GameUI;
using Network;
using Network.Models;
using Network.Models.Player;
using Player;
using ScenesSystem;
using Server.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private GameObject chatLayout;
    [SerializeField] private GameObject chatContainer;
    [SerializeField] private ChatItem chatItemExample;
    
    [SerializeField] private InputField chatField;
    [SerializeField] private Text chatFieldText;
    [SerializeField] private Dropdown messageTarget;

    [SerializeField] private Text statText;

    [SerializeField] private GameObject chatUI;
    [SerializeField] private PlayersListInRoomUI playersTableUI;

    [SerializeField] private Image bloodOverlayImage;
    
    private bool _isLeaved;
    private bool _isChatOpen;
    //private bool _isPlayersFieldOpen;

    private float lastMessageTime = 0f;
    
    private List<MessageModel> allMessages = new List<MessageModel>();
    
    void Start()
    {
        EventsManager<MessageReceivedEvent>.Register(OnMessageReceived);
        EventsManager<PlayersInRoomEvent>.Register(OnPlayersInRoomUpdated);
        EventsManager<LocalPlayerUpdateEvent>.Register(OnLocalPlayerUpdated);
        EventsManager<RespawnEvent>.Register(OnRespawn);
        NetworkManager.Instance.RequestAccessInRoom();
        
        // Hide by default
        playersTableUI.gameObject.SetActive(false);
        chatUI.SetActive(false);
        
        // Load scene
        ScenesManager.Instance.LoadLevel(NetworkManager.Instance.GetCurrentRoom().SceneName);
        
        chatLayout.SetActive(false);
    }

    private void OnDestroy()
    {
        EventsManager<LocalPlayerUpdateEvent>.Unregister(OnLocalPlayerUpdated);
        EventsManager<MessageReceivedEvent>.Unregister(OnMessageReceived);
        EventsManager<PlayersInRoomEvent>.Unregister(OnPlayersInRoomUpdated);
        EventsManager<RespawnEvent>.Unregister(OnRespawn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !PlayerController.Instance.NetworkSyncComponent.IsAlive)
            NetworkManager.Instance.Respawn();
        
        // Players filed show/hide
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //_isPlayersFieldOpen = true;
            playersTableUI.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            //_isPlayersFieldOpen = false;
            playersTableUI.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        // Chat show/hide toggle
        if (Input.GetKeyDown(KeyCode.U))
        {
            _isChatOpen = !_isChatOpen;
            chatUI.SetActive(_isChatOpen);
            
            PlayerController.Instance.SetInputLock(_isChatOpen);

            if (_isChatOpen)
            {
                ShowAllChatHistory();
            }
            else
            {
                chatLayout.SetActive(false);
            }
            
            Cursor.lockState = _isChatOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (!_isChatOpen && lastMessageTime != 0f && Time.time - lastMessageTime > 2.5f)
        {
            RemoveAllMessages();
            chatLayout.SetActive(false);
        }
        
        if (bloodOverlayAmount > 0f)
        {
            bloodOverlayAmount -= 0.06f;
        }
        else
        {
            bloodOverlayAmount = 0f;
        }
        SetBloodOverlay(bloodOverlayAmount);
    }

    private void OnRespawn(Vector3 pos, Vector3 rot)
    {
        Debug.Log("RESPAWN!");
        
        PlayerController.Instance.NetworkSyncComponent.SetIsAlive(true);
        
        PlayerController.Instance.GetCharacterController().enabled = false;
        Transform transform = PlayerController.Instance.gameObject.transform;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(rot);
        PlayerController.Instance.GetCharacterController().enabled = true;
    }

    private void UpdateStat(UpdateLocalPlayerInfo model)
    {
        statText.text = "";
        statText.text += $"HP: {model.Health}\n\n";
        statText.text += $"{model.CurrentWeapon.Id}\n";
        statText.text += $"{model.CurrentWeapon.Ammo} / {model.CurrentWeapon.AmmoReserve}";
    }

    private short prevHealth;
    public void OnLocalPlayerUpdated(UpdateLocalPlayerInfo model)
    {
        PlayerController.Instance.NetworkSyncComponent.SetIsAlive(!model.IsDead);
        
        PlayerController.Instance.PlayerWeaponComponent.UpdateByModel(model);
        UpdateStat(model);

        if(prevHealth > model.Health)
            OnDamage(prevHealth - model.Health);
        
        prevHealth = model.Health;
    }

    private float bloodOverlayAmount = 0f;
    
    private void OnDamage(int dmg)
    {
        bloodOverlayAmount = 1f;
    }

    private void SetBloodOverlay(float amount)
    {
        if (amount > 0f)
        {
            if (!bloodOverlayImage.enabled)
            {
                bloodOverlayImage.enabled = true;
            }
            bloodOverlayImage.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, amount));
        }
        else if (bloodOverlayImage.enabled)
        {
            bloodOverlayImage.enabled = false;
        }
    }

    private void OnPlayersInRoomUpdated(List<PlayerModel> players)
    {
        playersTableUI.UpdateList(players);
    }
    
    private void OnMessageReceived(MessageModel message)
    {
        if(_isLeaved) return;
        
        allMessages.Add(new MessageModel()
        {
            SenderNickname = message.SenderNickname,
            Type = message.Type,
            Text = message.Text
        });
        
        // if(!_isChatOpen) RemoveAllMessages();
        AddMessage(message);
        chatLayout.SetActive(true);

        lastMessageTime = Time.time;

        if (_isChatOpen)
        {
            StartCoroutine(ScrollToDown());
        }
    }

    private IEnumerator ScrollToDown()
    {
        yield return new WaitForSeconds(0.3f);
        ScrollRect scrollRect = chatLayout.GetComponentInChildren<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void RemoveAllMessages()
    {
        foreach (Transform listItemTransform in chatContainer.transform)
        {
            if(listItemTransform.gameObject != chatItemExample.gameObject)
                Destroy(listItemTransform.gameObject);
        }
    }

    private void AddMessage(MessageModel model)
    {
        GameObject newItem = Instantiate(
            chatItemExample.gameObject, 
            chatContainer.transform, 
            true
        );

        ChatItem newListItem = newItem.GetComponent<ChatItem>();
        newListItem.SetInfo(model);
            
        newItem.SetActive(true);
    }

    public void ShowAllChatHistory()
    {
        RemoveAllMessages();
        foreach (var msg in allMessages)
        {
            AddMessage(msg);
        }
        
        chatLayout.SetActive(true);
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
