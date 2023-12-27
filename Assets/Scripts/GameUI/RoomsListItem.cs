using System.Collections;
using System.Collections.Generic;
using Network.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomsListItem : MonoBehaviour
{
    public void SetInfo(RoomModel room)
    {
        Text nameText = GetComponentsInChildren<Text>()[0];
        Text plCountText = GetComponentsInChildren<Text>()[1];
        Text sceneText = GetComponentsInChildren<Text>()[2];
        Text gameModeText = GetComponentsInChildren<Text>()[3];

        nameText.text = room.Name;
        plCountText.text = $"{room.PlayerCount} / {room.PlayerMax}";
        sceneText.text = room.SceneName;
        gameModeText.text = room.GameMod.ToString();
    }
}