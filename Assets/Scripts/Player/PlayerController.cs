using System;
using System.Collections.Generic;
using System.Linq;
using Player.Components;

namespace Player
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        
        [SerializeField] private Transform[] groundChecks;
        
        public MouseControlComponent MouseControlComponent;
        public MovementComponent MovementComponent;
        public NetworkSyncComponent NetworkSyncComponent;
        public PlayerWeaponComponent PlayerWeaponComponent;
        
        private CharacterController _characterController;
        private Camera _playerCamera;
        private Camera _playerWeaponCamera;

        private bool _isInputLocked;
        
        public PlayerController()
        {
            Instance = this;
        }

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _playerCamera = GetComponentsInChildren<Camera>()[0];
            _playerWeaponCamera = GetComponentsInChildren<Camera>()[1];
        
            // Init components
            GameObject o;
            MouseControlComponent = new MouseControlComponent(GetMainCamera(), (o = gameObject));
            MovementComponent = new MovementComponent(
                o, GetCharacterController()
            );
            NetworkSyncComponent = new NetworkSyncComponent(o, GetMainCamera());
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

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            MovementComponent.OnControllerColliderHit(hit);
        }

        public void SetEnabled(bool isEnabled)
        {
            GetCharacterController().enabled = isEnabled;
        }

        public bool IsEnabled()
        {
            return GetCharacterController().enabled;
        }
        
        // public bool IsGrounded()
        // {
        // int ignoreRaycastLayer = LayerMask.NameToLayer("Player");
        // int layerMask = 1 << ignoreRaycastLayer;
        //     return Physics.CheckSphere(groundChecks.position, 0.2f, ~layerMask);
        // }

        public Camera GetWeaponCamera()
        {
            return _playerWeaponCamera;
        }

        public Camera GetMainCamera()
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
