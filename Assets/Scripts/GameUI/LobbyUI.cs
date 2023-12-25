using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private RoomsListItem exampleItem;
    [SerializeField] private GameObject roomsItemsContainer;
    
    void Start()
    {
        EventsManager<RoomsLoadedEvent>.Register((List<RoomModel> rooms) =>
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
                GameObject newItem = Instantiate(exampleItem.gameObject);

                RoomsListItem newListItem = newItem.GetComponent<RoomsListItem>();
                newListItem.GetTitleText().text = room.Name;
                newListItem.GetInfoeText().text = $"{room.PlayerCount} / {room.PlayerMax} in {room.SceneName} by {room.GameMod}";

                newItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    //NetworkManager.Instance.TryJoinRoom(room.ID);
                });
            
                newItem.SetActive(true);
                newItem.transform.SetParent(roomsItemsContainer.transform);
            }
        });

        RefreshRooms();
    }

    public void RefreshRooms()
    {
        NetworkManager.Instance.LoadRoomsList();
    }
}
