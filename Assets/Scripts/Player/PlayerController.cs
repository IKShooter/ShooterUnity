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
        public PlayerWeaponComponent PlayerWeaponComponent;
        
        private CharacterController _characterController;
        private Camera _playerCamera;

        private bool _isInputLocked;
        
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
            NetworkSyncComponent = new NetworkSyncComponent(gameObject, GetCamera());
            PlayerWeaponComponent = new PlayerWeaponComponent(this);
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // Don't do anything if controller disabled
            if(!_characterController.enabled || _isInputLocked) return;
        
            // Update all components
            MouseControlComponent.Update();
            MovementComponent.Update();
            NetworkSyncComponent.Update();
            PlayerWeaponComponent.Update();
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

        public bool IsInputLocked()
        {
            return _isInputLocked;
        }

        public void SetInputLock(bool isLocked)
        {
            _isInputLocked = isLocked;
        }
    }
}
