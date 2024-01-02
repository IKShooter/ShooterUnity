using System.Collections;
using Network;
using Network.Models;
using Server.Models;
using UnityEngine;

namespace Player.Components
{
    public class PlayerWeaponComponent : IControllerComponent
    {
        private readonly PlayerController _controller;

        private LocalPlayerWeaponModel _currentWeapon;

        private readonly WeaponPoint _weaponPoint;

        private bool _isShoot;
        private bool _isShootInProgress;

        public PlayerWeaponComponent(PlayerController controller)
        {
            _controller = controller;
            _weaponPoint = controller.GetWeaponCamera().gameObject.GetComponentInChildren<WeaponPoint>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                // TODO: Prevent reloading, if can't (no reserv or not needed)
                _isShoot = false;
                _weaponPoint.DoReload(() =>
                {
                    ReloadWeapon();
                    return false;
                });
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && (_currentWeapon.Ammo > 0 || _currentWeapon.Type == WeaponType.Melee))
            {
                if (!_weaponPoint.GetWeaponAnimIK().IsReload && !_isShootInProgress)
                {
                    _isShoot = true;
                    _controller.StartCoroutine(StartShootCoroutine());
                }
            } else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _isShoot = false;
            }

            _weaponPoint.UpdateIsMove(PlayerController.Instance.MovementComponent.IsMoving);
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SwitchWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SwitchWeapon(2);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                SwitchWeapon(3);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                SwitchWeapon(4);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                SwitchWeapon(5);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                SwitchWeapon(6);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                SwitchWeapon(7);
        }

        private IEnumerator StartShootCoroutine()
        {
            _isShootInProgress = true;
            
            while (_isShoot)
            {
                if (_currentWeapon.Ammo <= 0 && _currentWeapon.Type != WeaponType.Melee)
                {
                    _isShoot = false;
                    yield break;
                }
                
                _weaponPoint.DoShoot();
                Shoot();

                switch (_currentWeapon.Type)
                {
                    case WeaponType.Minigun:
                    case WeaponType.AssaultRifle:
                        yield return new WaitForSeconds(1f / _currentWeapon.FireRate);
                        break;
                    
                    default:
                        _isShoot = false;
                        yield break;
                }
            }

            _isShootInProgress = false;
        }
        
        private void Shoot()
        {
            GameObject weaponGameObject = _weaponPoint.GetWeaponGameObject();
            weaponGameObject.GetComponentInChildren<ShootEmitter>()?.Emit();
            
            // int ignoreRaycastLayer = LayerMask.NameToLayer("Player");
            // int layerMask = 1 << ignoreRaycastLayer;

            bool isMelee = _currentWeapon.Type == WeaponType.Melee;

            bool isHitted = Physics.Raycast(
                _controller.GetMainCamera().gameObject.transform.position + _controller.GetMainCamera().gameObject.transform.forward * 1f,
                _controller.GetMainCamera().gameObject.transform.forward,
                out var hit,
                isMelee ? 1f : 1000f,
                1
            );

            bool isHitRemotePlayer = isHitted && hit.collider.gameObject.name.Contains("EnemyPlayer");

            NetworkManager.Instance.TryShoot(
                isHitted ? hit.point : Vector3.zero, 
                isHitRemotePlayer,
                isHitRemotePlayer ? hit.collider.gameObject.gameObject.GetComponentInChildren<RemotePlayerComponent>().PlayerModel.Id : 0
            );
            
            if (isHitRemotePlayer)
                Debug.Log($"Hitted enemy! {hit.collider.gameObject.name}");

            if (_currentWeapon.Type == WeaponType.Shotgun)
            {
                // TODO
                MakeShootImpulse();
            }
        }

        private void MakeShootImpulse()
        {
            CharacterController characterController = _controller.GetCharacterController();
            var transform = _controller.transform;
            Vector3 impulseDirection = -transform.forward * 3.5f + transform.up * 5.5f; // Adjust the impulse direction and magnitude as needed
            float impulseForce = 5.0f; // Adjust the force magnitude as needed

            // Apply the impulse using Move
            characterController.Move(impulseDirection * (impulseForce * Time.deltaTime));
        }


        private void ReloadWeapon()
        {
            NetworkManager.Instance.TryReloadWeapon();
        }

        private void SwitchWeapon(byte slotId)
        {
            NetworkManager.Instance.TrySwitchWeapon(slotId);
        }

        public void UpdateByModel(UpdateLocalPlayerInfo model)
        {
            _currentWeapon = model.CurrentWeapon;
            _weaponPoint.SetActiveWeapon(_currentWeapon);
        }
    }
}