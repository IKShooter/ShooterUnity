using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class PlayerListItem : MonoBehaviour
    {
        public void SetPlayerInfo(PlayerModel player)
        {
            GetComponentsInChildren<Text>()[0].text = $"{player.Nickname} ({player.Id})";
            // GetComponentsInChildren<Text>()[1].text = "";
        }
    }
}