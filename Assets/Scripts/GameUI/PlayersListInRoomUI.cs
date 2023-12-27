using System.Collections.Generic;
using Network.Models;
using UnityEngine;

namespace GameUI
{
    public class PlayersListInRoomUI : MonoBehaviour
    {
        [SerializeField] private GameObject playersItemsContainer;
        [SerializeField] private GameObject exampleItem;
        
        public void UpdateList(List<PlayerModel> players)
        {
            // Remove old items
            foreach (var listItem in playersItemsContainer.GetComponentsInChildren<RoomsListItem>())
            {
                if(listItem != exampleItem)
                    Destroy(listItem.gameObject);
            }
            
            // Add new items
            foreach (var pl in players)
            {
                GameObject newItem = Instantiate(
                    exampleItem.gameObject, 
                    playersItemsContainer.transform, 
                    true
                );

                PlayerListItem newListItem = newItem.GetComponent<PlayerListItem>();
                newListItem.SetInfo(pl);
            
                newItem.SetActive(true);
            }
        }
    }
}