using System.Collections.Generic;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class PlayersFieldUI : MonoBehaviour
    {
        [SerializeField] private RoomsListItem exampleItem;
        [SerializeField] private GameObject itemsContainer;

        public void UpdateList(List<PlayerModel> players)
        {
            // Remove old items
            foreach (var listItem in itemsContainer.GetComponentsInChildren<PlayerListItem>())
            {
                if(listItem != exampleItem)
                    Destroy(listItem.gameObject);
            }
            
            // Add new items
            foreach (var player in players)
            {
                GameObject newItem = Instantiate(
                    exampleItem.gameObject, 
                    itemsContainer.transform, 
                    true
                );

                PlayerListItem newListItem = newItem.GetComponent<PlayerListItem>();
                newListItem.SetPlayerInfo(player);

                newItem.GetComponentInChildren<Button>().onClick.AddListener(
                    () => NetworkManager.Instance.TryJoinRoom(player.Nickname));
            
                newItem.SetActive(true);
            }
        }
    }
}