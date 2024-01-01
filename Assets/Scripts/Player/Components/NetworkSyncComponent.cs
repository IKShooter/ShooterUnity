using System;
using Network;
using UnityEngine;

namespace Player.Components
{
    public class NetworkSyncComponent : IControllerComponent
    {
        private readonly GameObject _playerBody;
        private readonly Camera _camera;
        
        private float _lastNetworkUpdate;
        
        private Vector3 _lastPos;
        private float _lastRotY;

        private bool _isAlive;

        public bool IsAlive
        {
            get => _isAlive;
        }

        public NetworkSyncComponent(GameObject body, Camera camera)
        {
            _playerBody = body;
            _camera = camera;
        }
        

        public void SetIsAlive(bool isAlive)
        {
            _isAlive = isAlive;
            PlayerController.Instance.GetCharacterController().enabled = _isAlive;
        }
        
        public void Update()
        {
            if (!_isAlive) return;
            
            if (Time.time - _lastNetworkUpdate > 1f / GameConstants.PlayerSyncTps)
            {
                Transform transform = _playerBody.transform;

                if (_lastPos != transform.position || Math.Abs(_lastRotY - transform.rotation.eulerAngles.y) > 0.1f)
                {
                    var position = transform.position;
                    var rotation = transform.rotation;
                    NetworkManager.Instance.UpdatePlayer(
                        position, 
                        rotation.eulerAngles.y, 
                        _camera.transform.rotation.eulerAngles.x
                    );
                    _lastPos = position;
                    _lastRotY = rotation.eulerAngles.y;
                }
                _lastNetworkUpdate = Time.time;
            }
        }
    }
}