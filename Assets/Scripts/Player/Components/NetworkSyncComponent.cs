using UnityEngine;

namespace Player.Components
{
    public class NetworkSyncComponent : IControllerComponent
    {
        private GameObject _playerBody;
        private Camera _camera;
        
        private float _lastNetworkUpdate;

        private bool _isAlive;

        public NetworkSyncComponent(GameObject body, Camera camera)
        {
            _playerBody = body;
            _camera = camera;
        }
        
        private Vector3 lastPos;
        private float lastRotY;

        public void SetIsAlive(bool isAlive)
        {
            _isAlive = isAlive;
            PlayerController.Instance.GetCharacterController().enabled = _isAlive;
        }
        
        public void Update()
        {
            if(_isAlive)
            if (Time.time - _lastNetworkUpdate > 1f / GameConstants.PlayerSyncTps)
            {
                Transform transform = _playerBody.transform;

                if (lastPos != transform.position || lastRotY != transform.rotation.eulerAngles.y)
                {
                    NetworkManager.Instance.UpdatePlayer(
                        transform.position, 
                        transform.rotation.eulerAngles.y, 
                        _camera.transform.rotation.eulerAngles.x
                        );
                    lastPos = transform.position;
                    lastRotY = transform.rotation.eulerAngles.y;
                }
                _lastNetworkUpdate = Time.time;
            }
        }
    }
}