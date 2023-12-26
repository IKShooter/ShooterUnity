using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class RemotePlayer
    {
        public PlayerModel _model;
        public GameObject _gameObject;

        public RemotePlayer(PlayerModel model, GameObject gameObject)
        {
            _model = model;
            _gameObject = gameObject;
        }
    }
    
    public class RemotePlayersController : MonoBehaviour
    {
        public RemotePlayersController Instance;

        [SerializeField] private GameObject playersParentGameObject;
        
        public RemotePlayersController()
        {
            Instance = this;
        }

        private List<RemotePlayer> _remotePlayers = new List<RemotePlayer>();

        private void Start()
        {
            EventsManager<PlayersInRoomEvent>.Register((models =>
            {
                Debug.Log($"PlayersInRoomEvent {models.Count}");
                
                foreach (var model in models)
                {
                    if(NetworkManager.Instance.GetCurrentPlayer().Id == model.Id)
                        continue;
                    
                    RemotePlayer remotePlayer = _remotePlayers.Find(pl => pl._model.Id == model.Id);
                    if (remotePlayer == null)
                    {
                        GameObject newPlayerObject = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyPlayer"));
                        
                        remotePlayer = new RemotePlayer(model, newPlayerObject);
                        _remotePlayers.Add(remotePlayer);
                        
                        newPlayerObject.transform.SetParent(playersParentGameObject.transform);
                        
                        Debug.Log($"Registered new player! {model.Id}");
                    }
                    else
                    {
                        remotePlayer._model.RotationY = model.RotationY;
                        remotePlayer._model.Position = model.Position;
                    }
                }
            }));
            
            EventsManager<UpdatePlayerInRoomEvent>.Register((model =>
            {
                Debug.Log($"UpdatePlayerInRoomEvent for {model.Id}");
                
                RemotePlayer remotePlayer = _remotePlayers.Find(pl => pl._model.Id == model.Id);

                if (remotePlayer != null)
                {
                    remotePlayer._model.RotationY = model.RotationY;
                    remotePlayer._model.Position = model.Position;
                }
            }));
        }

        private float interpolationSpeed = 4.5f;
        
        private void Update()
        {
            foreach (var remotePlayer in _remotePlayers)
            {
                remotePlayer._gameObject.transform.position = Vector3.Lerp(remotePlayer._gameObject.transform.position, remotePlayer._model.Position, Time.deltaTime*interpolationSpeed);
                remotePlayer._gameObject.transform.rotation = Quaternion.Slerp(remotePlayer._gameObject.transform.rotation, Quaternion.Euler(0f, remotePlayer._model.RotationY, 0f), Time.deltaTime*interpolationSpeed);
            }
        }
    }
}