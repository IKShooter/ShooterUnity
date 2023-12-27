using UnityEngine;

namespace Player.Components
{
    public class NetworkSyncComponent : IControllerComponent
    {
        private GameObject _playerBody;
        
        private float _lastNetworkUpdate = 0f;

        public NetworkSyncComponent(GameObject body)
        {
            _playerBody = body;
        }
        
        private Vector3 lastPos;
        private float lastRotY;
        
        public void Update()
        {
            if (Time.time - _lastNetworkUpdate > 1f / GameConstants.PlayerSyncTps)
            {
                Transform transform = _playerBody.transform;

                if (lastPos != transform.position || lastRotY != transform.rotation.eulerAngles.y)
                {
                    NetworkManager.Instance.UpdatePlayer(transform.position, transform.rotation.eulerAngles.y);
                    lastPos = transform.position;
                    lastRotY = transform.rotation.eulerAngles.y;
                }
                _lastNetworkUpdate = Time.time;
            }
        }
    }
}