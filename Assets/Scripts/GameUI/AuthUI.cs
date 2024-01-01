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
        
        private void Start()
        {
            nicknameText.text = PlayerPrefs.GetString("LastNickname");
            
            authButton.interactable = false;
            anonAuthButton.interactable = false;
            
            EventsManager<ServerConnectedEvent>.Register(OnServerConnected);
            EventsManager<SuccessAuthEvent>.Register(OnSuccessAuth);
        }

        private void OnDestroy()
        {
            EventsManager<ServerConnectedEvent>.Unregister(OnServerConnected);
            EventsManager<SuccessAuthEvent>.Unregister(OnSuccessAuth);
        }

        private void OnServerConnected(NetPeer peer)
        {
            authButton.interactable = true;
            anonAuthButton.interactable = true;
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

        public void TryAuthAnonimous()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            
            NetworkManager.Instance.TryAuth(finalString);
        }
    }
}