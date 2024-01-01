using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class PlayerListItem : MonoBehaviour
    {
        public void SetInfo(PlayerModel player)
        {
            GetComponentsInChildren<Text>()[0].text = $"{player.Nickname} ({player.Id})";
            GetComponentsInChildren<Text>()[1].text = "-";
            GetComponentsInChildren<Text>()[2].text = "-";
            GetComponentsInChildren<Text>()[3].text = player.Ping.ToString();
        }
    }
}