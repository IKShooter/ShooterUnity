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
        public RemoteCameraJoin cameraJoin;
        public WeaponPoint WeaponPoint;
        public bool isDead;
        
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
        
        private float interpolationSpeed = 5.0f;
        
        public RemotePlayersController()
        {
            Instance = this;
        }

        private List<RemotePlayer> _remotePlayers = new List<RemotePlayer>();

        private void Start()
        {
            EventsManager<PlayerShootEvent>.Register(model =>
            {
                // Is not full missed
                if(model.PosTo != Vector3.zero)
                    Utils.SpawnHitParticle(model.PosTo);
                
                if(model.IsHit)
                    Debug.Log($"SHOOTING! {model.PlayerShooter.Nickname}"); // TODO: to {model.TargetPlayer.Nickname}"
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

                        remotePlayer.cameraJoin = newPlayerObject.GetComponentInChildren<RemoteCameraJoin>();

                        remotePlayer.WeaponPoint = newPlayerObject.GetComponentInChildren<WeaponPoint>();
                        
                        newPlayerObject.transform.SetParent(playersParentGameObject.transform);

                        remotePlayer._gameObject.transform.position = remotePlayer._model.Position;
                        remotePlayer._gameObject.transform.rotation = Quaternion.Euler(0f, remotePlayer._model.RotationY, 0f);
                        
                        Debug.Log($"Registered new player! {model.Id}");
                    }
                    else
                    {
                        remotePlayer._model.RotationY = model.RotationY;
                        remotePlayer._model.RotationCameraX = model.RotationCameraX;
                        remotePlayer._model.Position = model.Position;
                    }
                }
                
                // Track leaved players
                foreach (RemotePlayer remotePlayer in _remotePlayers)
                {
                    // Is leaved
                    if (models.Find(model => model.Id == remotePlayer._model.Id) == null)
                    {
                        EventsManager<RemotePlayerLeaveEvent>.Trigger?.Invoke(remotePlayer._model);
                        Destroy(remotePlayer._gameObject);
                        // Debug.Log($"CURVA, DESTROYED!!! {remotePlayer._model.Nickname}");
                        _remotePlayers.Remove(remotePlayer);
                        break;
                    }
                }
            }));
            
            EventsManager<UpdatePlayerInRoomEvent>.Register((model =>
            {
                foreach (var updatePlayerInRoom in model.updates)
                {
                    // Debug.Log($"UpdatePlayerInRoomEvent for {updatePlayerInRoom.Id} ({updatePlayerInRoom.Position.y})");
                
                    RemotePlayer remotePlayer = _remotePlayers.Find(pl => pl._model.Id == updatePlayerInRoom.Id);

                    if (remotePlayer != null)
                    {
                        remotePlayer._model.RotationY = updatePlayerInRoom.RotationY;
                        remotePlayer._model.RotationCameraX = updatePlayerInRoom.RotationCameraX;
                        remotePlayer._model.Position = updatePlayerInRoom.Position;
                        remotePlayer._model.Ping = updatePlayerInRoom.Ping;
                        remotePlayer.isDead = updatePlayerInRoom.IsDead;
                        
                        remotePlayer.WeaponPoint.SetActiveWeapon(updatePlayerInRoom.CurrentWeapon);
                    }
                }
            }));
        }

        
        private void Update()
        {
            foreach (var remotePlayer in _remotePlayers)
            {
                remotePlayer._gameObject.SetActive(!remotePlayer.isDead);
                if(remotePlayer.isDead) // Skip player update is dead
                    continue;
                
                remotePlayer._gameObject.transform.position = Vector3.Lerp(remotePlayer._gameObject.transform.position, remotePlayer._model.Position, Time.deltaTime*interpolationSpeed);
                remotePlayer._gameObject.transform.rotation = Quaternion.Slerp(remotePlayer._gameObject.transform.rotation, Quaternion.Euler(0f, remotePlayer._model.RotationY, 0f), Time.deltaTime*interpolationSpeed);
                remotePlayer.cameraJoin.transform.rotation = Quaternion.Slerp(
                    remotePlayer.cameraJoin.transform.rotation, 
                    Quaternion.Euler(
                        remotePlayer._model.RotationCameraX, 
                        remotePlayer.cameraJoin.transform.rotation.y,
                        remotePlayer.cameraJoin.transform.rotation.z
                    ),
                Time.deltaTime*interpolationSpeed
                );
                
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