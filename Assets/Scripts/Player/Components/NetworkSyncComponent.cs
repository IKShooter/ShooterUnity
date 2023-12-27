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
        
        public void Update()
        {
            if (Time.time - _lastNetworkUpdate > 1_000 / GameConstants.PlayerSyncTps)
            {
                Transform transform = _playerBody.transform;
                NetworkManager.Instance.UpdatePlayer(transform.position, transform.rotation.eulerAngles.y);
                _lastNetworkUpdate = Time.time;
            }
        }
    }
}