using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreateUI : MonoBehaviour
{
    [SerializeField] private Text roomNameField;
    [SerializeField] private Dropdown maxPlayersSelector;
    [SerializeField] private Dropdown sceneSelector;
    [SerializeField] private Dropdown gameModeSelector;

    public void TryCreateRoom()
    {
        int maxPlayers = Int32.Parse(maxPlayersSelector.options[maxPlayersSelector.value].text);
        string sceneName = sceneSelector.options[sceneSelector.value].text;
        string gameMode = gameModeSelector.options[gameModeSelector.value].text;

        NetworkManager.Instance.TryCreateRoom(roomNameField.text, maxPlayers, sceneName, gameMode);
    }
}
