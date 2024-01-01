using System.Collections.Generic;
using Events;
using Network.Enums;
using Network.Models;
using Network.Models.Player;
using UnityEngine;

namespace Player
{
    public class RemotePlayer
    {
        public PlayerModel Model;
        public GameObject GameObject;
        public RemoteCameraJoin CameraJoin;
        public WeaponPoint WeaponPoint;
        public bool IsDead;
        
        public RemotePlayer(PlayerModel model, GameObject gameObject)
        {
            Model = model;
            GameObject = gameObject;
        }
    }
    
    public class RemotePlayersController : MonoBehaviour
    {
        public RemotePlayersController instance;

        [SerializeField] private GameObject playersParentGameObject;
        
        private float _interpolationSpeed = 5.0f;
        
        public RemotePlayersController()
        {
            instance = this;
        }

        private List<RemotePlayer> _remotePlayers = new List<RemotePlayer>();

        private void Start()
        {
            EventsManager<OtherDamageEvent>.Register(OnOtherDamageEvent);
            EventsManager<PlayerShootEvent>.Register(OnPlayerShootEvent);
            EventsManager<PlayersInRoomEvent>.Register(OnPlayersInRoomEvent);
            EventsManager<UpdatePlayerInRoomEvent>.Register(OnUpdatePlayersInRoomEvent);
        }
        
        private void OnDestroy()
        {
            EventsManager<OtherDamageEvent>.Unregister(OnOtherDamageEvent);
            EventsManager<PlayerShootEvent>.Unregister(OnPlayerShootEvent);
            EventsManager<PlayersInRoomEvent>.Unregister(OnPlayersInRoomEvent);
            EventsManager<UpdatePlayerInRoomEvent>.Unregister(OnUpdatePlayersInRoomEvent);
        }

        private void OnOtherDamageEvent(DamageInfoModel model)
        {
            foreach (var remotePlayer in _remotePlayers)
            {
                if (remotePlayer.Model.Id == model.PlayerHitedId)
                {
                    Color color = Color.magenta;
                    switch (model.DamageType)
                    {
                        case DamageType.Standart:
                            color = Color.white;
                            break;
                        case DamageType.Critical:
                            color = Color.red;
                            break;
                        case DamageType.Headshot:
                            color = Color.yellow;
                            break;
                    }
                    remotePlayer.GameObject.GetComponentInChildren<DmgNumberEmitter>().SpawnNumber(model.Damage, color);
                    break;
                }
            }
        }
        
        private void OnPlayerShootEvent(ShootModel model)
        {
            // Is not full missed
            if(model.PosTo != Vector3.zero)
                Utils.SpawnHitParticle(model.PosTo);

            Debug.Log(model.IsHit
                ? $"SHOOTING! {model.PlayerShooter.Nickname}"
                : $"MISS SHOOT BY {model.PlayerShooter.Nickname}"); // TODO: to {model.TargetPlayer.Nickname}"
        }

        private void OnPlayersInRoomEvent(List<PlayerModel> models)
        {
            Debug.Log($"PlayersInRoomEvent {models.Count}");
                
                foreach (var model in models)
                {
                    if(NetworkManager.Instance.GetCurrentPlayer().Id == model.Id)
                        continue;
                    
                    RemotePlayer remotePlayer = _remotePlayers.Find(pl => pl.Model.Id == model.Id);
                    if (remotePlayer == null)
                    {
                        GameObject newPlayerObject = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyPlayer"), playersParentGameObject.transform, true);
                        
                        TextMesh nickNameText = newPlayerObject.GetComponentInChildren<TextMesh>();
                        nickNameText.text = model.Nickname;
                        
                        remotePlayer = new RemotePlayer(model, newPlayerObject);
                        _remotePlayers.Add(remotePlayer);
                        
                        // Disable nickname
                        remotePlayer.GameObject.GetComponentInChildren<TextMesh>().gameObject.SetActive(false);

                        // Assign player data
                        newPlayerObject.AddComponent<RemotePlayerComponent>().playerModel = model;

                        remotePlayer.CameraJoin = newPlayerObject.GetComponentInChildren<RemoteCameraJoin>();

                        remotePlayer.WeaponPoint = newPlayerObject.GetComponentInChildren<WeaponPoint>();

                        remotePlayer.GameObject.transform.position = remotePlayer.Model.Position;
                        remotePlayer.GameObject.transform.rotation = Quaternion.Euler(0f, remotePlayer.Model.RotationY, 0f);
                        
                        Debug.Log($"Registered new player! {model.Id}");
                    }
                    else
                    {
                        remotePlayer.Model.RotationY = model.RotationY;
                        remotePlayer.Model.RotationCameraX = model.RotationCameraX;
                        remotePlayer.Model.Position = model.Position;
                    }
                }
                
                // Track leaved players
                foreach (RemotePlayer remotePlayer in _remotePlayers)
                {
                    // Is leaved
                    if (models.Find(model => model.Id == remotePlayer.Model.Id) == null)
                    {
                        EventsManager<RemotePlayerLeaveEvent>.Trigger?.Invoke(remotePlayer.Model);
                        Destroy(remotePlayer.GameObject);
                        // Debug.Log($"CURVA, DESTROYED!!! {remotePlayer._model.Nickname}");
                        _remotePlayers.Remove(remotePlayer);
                        break;
                    }
                }
        }

        private void OnUpdatePlayersInRoomEvent(UpdatePlayersTickInRoom model)
        {
            foreach (var updatePlayerInRoom in model.updates)
            {
                // Debug.Log($"UpdatePlayerInRoomEvent for {updatePlayerInRoom.Id} ({updatePlayerInRoom.Position.y})");
                
                RemotePlayer remotePlayer = _remotePlayers.Find(pl => pl.Model.Id == updatePlayerInRoom.Id);

                if (remotePlayer != null)
                {
                    remotePlayer.Model.RotationY = updatePlayerInRoom.RotationY;
                    remotePlayer.Model.RotationCameraX = updatePlayerInRoom.RotationCameraX;
                    remotePlayer.Model.Position = updatePlayerInRoom.Position;
                    remotePlayer.Model.Ping = updatePlayerInRoom.Ping;
                    remotePlayer.IsDead = updatePlayerInRoom.IsDead;
                        
                    remotePlayer.WeaponPoint.SetActiveWeapon(updatePlayerInRoom.CurrentWeapon);
                }
            }
        }
        
        private void Update()
        {
            foreach (var remotePlayer in _remotePlayers)
            {
                remotePlayer.GameObject.SetActive(!remotePlayer.IsDead);
                if(remotePlayer.IsDead) // Skip player update is dead
                    continue;
                
                remotePlayer.GameObject.transform.position = Vector3.Lerp(remotePlayer.GameObject.transform.position, remotePlayer.Model.Position, Time.deltaTime*_interpolationSpeed);
                remotePlayer.GameObject.transform.rotation = Quaternion.Slerp(remotePlayer.GameObject.transform.rotation, Quaternion.Euler(0f, remotePlayer.Model.RotationY, 0f), Time.deltaTime*_interpolationSpeed);
                var rotation = remotePlayer.CameraJoin.transform.rotation;
                rotation = Quaternion.Slerp(
                    rotation, 
                    Quaternion.Euler(
                        remotePlayer.Model.RotationCameraX, 
                        rotation.y,
                        rotation.z
                    ),
                Time.deltaTime*_interpolationSpeed
                );
                remotePlayer.CameraJoin.transform.rotation = rotation;

                // Rotate nick
                TextMesh nickNameText = remotePlayer.GameObject.GetComponentInChildren<TextMesh>();
                if (nickNameText)
                {
                    GameObject nickNameTextGameObject = nickNameText.gameObject;
                    GameObject go = PlayerController.Instance.GetMainCamera().gameObject;
                    var position = nickNameTextGameObject.transform.position;
                    Vector3 heading = go.transform.position - position;
                    nickNameTextGameObject.transform.LookAt(position - heading);
                }
            }
        }
    }
}