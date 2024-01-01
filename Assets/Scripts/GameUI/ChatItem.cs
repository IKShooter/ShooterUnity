using Network;
using Server.Models;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class ChatItem : MonoBehaviour
    {
        public void SetInfo(MessageModel model)
        {
            GetComponentsInChildren<Text>()[0].text = (model.Type == TypeMessage.System) ? model.Text : $"{model.SenderNickname} : {model.Text}";
        }
    }
}