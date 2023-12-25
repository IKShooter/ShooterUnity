using System;
using Events;
using UnityEngine;
using UnityEngine.UI;

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
        }

        public void TryAuth()
        {
            NetworkManager.Instance.TryAuth(nicknameText.text);
        }        
    }
}