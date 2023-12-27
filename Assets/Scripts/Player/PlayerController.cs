using Player.Components;

namespace Player
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        
        [SerializeField] private Transform groundCheck;
        
        public MouseControlComponent MouseControlComponent;
        public MovementComponent MovementComponent;
        public NetworkSyncComponent NetworkSyncComponent;
        
        private CharacterController _characterController;
        private Camera _playerCamera;

        public PlayerController()
        {
            Instance = this;
        }

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _playerCamera = GetComponentInChildren<Camera>();
        
            // Init components
            MouseControlComponent = new MouseControlComponent(_playerCamera, gameObject);
            MovementComponent = new MovementComponent(_characterController, gameObject);
            NetworkSyncComponent = new NetworkSyncComponent(gameObject);
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // Don't do anything if controller disabled
            if(!_characterController.enabled) return;
        
            // Update all components
            MouseControlComponent.Update();
            MovementComponent.Update();
        }
        
        public bool IsGrounded()
        {
            int ignoreRaycastLayer = LayerMask.NameToLayer("Player");
            int layerMask = 1 << ignoreRaycastLayer;
            return Physics.CheckSphere(groundCheck.position, 0.2f, ~layerMask);
        }

        public Camera GetCamera()
        {
            return _playerCamera;
        }

        public CharacterController GetCharacterController()
        {
            return _characterController;
        }
    }
}
