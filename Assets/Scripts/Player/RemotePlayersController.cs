using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            EventsManager<PlayerShootEvent>.Register(model =>
            {
                if(model.IsHit)
                    Debug.Log($"SHOOTING! {model.PlayerShooter.Nickname} to {model.TargetPlayer.Nickname}");
                else
                    Debug.Log($"MISS SHOOT BY {model.PlayerShooter.Nickname}");
            });
            
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
                        
                        TextMesh nickNameText = newPlayerObject.GetComponentInChildren<TextMesh>();
                        nickNameText.text = model.Nickname;
                        
                        remotePlayer = new RemotePlayer(model, newPlayerObject);
                        _remotePlayers.Add(remotePlayer);

                        // Assign player data
                        newPlayerObject.AddComponent<RemotePlayerComponent>().playerModel = model;
                        
                        newPlayerObject.transform.SetParent(playersParentGameObject.transform);

                        remotePlayer._gameObject.transform.position = remotePlayer._model.Position;
                        remotePlayer._gameObject.transform.rotation = Quaternion.Euler(0f, remotePlayer._model.RotationY, 0f);
                        
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
                foreach (var updatePlayerInRoom in model.updates)
                {
                    // Debug.Log($"UpdatePlayerInRoomEvent for {updatePlayerInRoom.Id}");
                
                    RemotePlayer remotePlayer = _remotePlayers.Find(pl => pl._model.Id == updatePlayerInRoom.Id);

                    if (remotePlayer != null)
                    {
                        remotePlayer._model.RotationY = updatePlayerInRoom.RotationY;
                        remotePlayer._model.Position = updatePlayerInRoom.Position;
                    }
                }
            }));
        }

        private float interpolationSpeed = 5.0f;
        
        private void Update()
        {
            foreach (var remotePlayer in _remotePlayers)
            {
                remotePlayer._gameObject.transform.position = Vector3.Lerp(remotePlayer._gameObject.transform.position, remotePlayer._model.Position, Time.deltaTime*interpolationSpeed);
                remotePlayer._gameObject.transform.rotation = Quaternion.Slerp(remotePlayer._gameObject.transform.rotation, Quaternion.Euler(0f, remotePlayer._model.RotationY, 0f), Time.deltaTime*interpolationSpeed);
                
                // Without interpolation
                // remotePlayer._gameObject.transform.position = remotePlayer._model.Position;
                // remotePlayer._gameObject.transform.rotation = Quaternion.Euler(0f, remotePlayer._model.RotationY, 0f);
                
                // Rotate nick
                GameObject nickNameTextGameObject = remotePlayer._gameObject.GetComponentInChildren<TextMesh>().gameObject;
                if (nickNameTextGameObject)
                {
                    GameObject go = PlayerController.Instance.GetCamera().gameObject;
                    Vector3 heading = go.transform.position - nickNameTextGameObject.transform.position;
                    nickNameTextGameObject.transform.LookAt(nickNameTextGameObject.transform.position - heading);
                }
            }
        }
    }
}