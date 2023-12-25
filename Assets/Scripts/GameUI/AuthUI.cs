using System;
using Events;
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
        
        private void Start()
        {
            authButton.interactable = false;
            
            EventsManager<ServerConnectedEvent>.Register(peer =>
            {
                authButton.interactable = true;
            });
            
            EventsManager<SuccessAuthEvent>.Register(() =>
            {
                SceneManager.LoadScene("Scenes/MainMenu");
            });
        }

        public void TryAuth()
        {
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