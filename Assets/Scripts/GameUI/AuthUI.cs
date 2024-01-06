using System;
using Events;
using LiteNetLib;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace GameUI
{
    public class AuthUI : MonoBehaviour
    {
        [SerializeField] private Text nicknameText;
        [SerializeField] private Button authButton;
        [SerializeField] private Button anonAuthButton;

        [SerializeField] private Text stateText;

        private enum State {
            Undefined,
            Connecting,
            Connected,
            Disconnected
        }
        
        private void SetState(State state) {
            stateText.text = state.ToString();
            if(state == State.Connected)
                stateText.text = "";

            authButton.interactable = state == State.Connected;
            anonAuthButton.interactable = state == State.Connected;
        }
        
        private void Start()
        {
            SetState(State.Connecting);

            nicknameText.text = PlayerPrefs.GetString("LastNickname");
            
            EventsManager<ServerDisconnectedEvent>.Register(OnServerDisconnected);
            EventsManager<ServerConnectedEvent>.Register(OnServerConnected);
            EventsManager<SuccessAuthEvent>.Register(OnSuccessAuth);
        }

        private void OnDestroy()
        {
            EventsManager<ServerDisconnectedEvent>.Unregister(OnServerDisconnected);
            EventsManager<ServerConnectedEvent>.Unregister(OnServerConnected);
            EventsManager<SuccessAuthEvent>.Unregister(OnSuccessAuth);
        }

        private void OnServerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            SetState(State.Disconnected);
        }

        private void OnServerConnected(NetPeer peer)
        {
            SetState(State.Connected);
        }

        private void OnSuccessAuth()
        {
            SceneManager.LoadScene("Scenes/MainMenu");
        }

        public void TryAuth()
        {
            PlayerPrefs.SetString("LastNickname", nicknameText.text);
            PlayerPrefs.Save();
            
            NetworkManager.Instance.TryAuth(nicknameText.text);
        }

        private string GetRandomNickname() {
            var random = new Random();

            var leftSides = new string[]{ "Белый", "Суб", "Желтый", "Демон", "Манящий", "Крутой", "Жованый" };
            var rightSides = new string[]{ "Крот", "Блин", "Кот", "Вор", "Брат", "Арбуз" };
            var value = random.Next(0, 7777).ToString();
            return $"{leftSides[random.Next(0, leftSides.Length-1)]}{rightSides[random.Next(0, rightSides.Length-1)]}{value}";
        }

        public void TryAuthAnonimous()
        {
            NetworkManager.Instance.TryAuth(GetRandomNickname());
        }
    }
}