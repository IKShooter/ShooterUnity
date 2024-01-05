using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Network;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private RoomsListItem exampleItem;
    [SerializeField] private GameObject roomsItemsContainer;

    [SerializeField] private GameObject roomListUI;
    [SerializeField] private GameObject createRoomUI;

    private void OnDestroy()
    {
        Debug.Log("LobbyUI::OnDestroy()");
        
        EventsManager<RoomsLoadedEvent>.Unregister(OnRoomList);
        EventsManager<RoomJoinedEvent>.Unregister(OnRoomJoined);
    }
    
    void Start()
    {
        EventsManager<RoomsLoadedEvent>.Register(OnRoomList);
        EventsManager<RoomJoinedEvent>.Register(OnRoomJoined);

        RefreshRooms();
    }
    private void OnRoomList(List<RoomModel> rooms)
    {  
        Debug.Log($"Rooms list received {rooms.Count}!");
            
        // Remove old items
        foreach (var listItem in roomsItemsContainer.GetComponentsInChildren<RoomsListItem>())
        {
            if(listItem != exampleItem)
                Destroy(listItem.gameObject);
        }
            
        // Add new items
        foreach (var room in rooms)
        {
            GameObject newItem = Instantiate(
                exampleItem.gameObject, 
                roomsItemsContainer.transform, 
                true
            );

            RoomsListItem newListItem = newItem.GetComponent<RoomsListItem>();
            newListItem.SetInfo(room);

            newItem.GetComponentInChildren<Button>().onClick.AddListener(
                () => NetworkManager.Instance.TryJoinRoom(room.Name));
            
            newItem.SetActive(true);
        }
    }

    private void OnRoomJoined(RoomModel room)
    {
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene("Scenes/InGame");
    }

    public void OpenRoomsList()
    {
        roomListUI.SetActive(true);
        createRoomUI.SetActive(false);
    }

    public void OpenCreateRoom()
    {
        roomListUI.SetActive(false);
        createRoomUI.SetActive(true);
    }

    public void RefreshRooms()
    {
        NetworkManager.Instance.LoadRoomsList();
    }

    public void QuitGame()
    {
        NetworkManager.Instance.TryDisconnect();
        Application.Quit();
    }
}
