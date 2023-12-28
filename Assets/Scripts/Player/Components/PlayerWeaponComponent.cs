using System;
using System.Collections.Generic;
using Network.Models;
using Server.Models;
using UnityEngine;

namespace Player.Components
{
    public class PlayerWeaponComponent : IControllerComponent
    {
        private PlayerController _controller;

        //private List<LocalPlayerWeaponModel> weapons;
        private LocalPlayerWeaponModel currentWeapon;

        private WeaponPoint _weaponPoint;

        public PlayerWeaponComponent(PlayerController controller)
        {
            _controller = controller;
            _weaponPoint = controller.GetCamera().gameObject.GetComponentInChildren<WeaponPoint>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                ReloadWeapon();
            
            if(Input.GetKeyDown(KeyCode.Mouse0))
                Shoot();
            
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

        private void Shoot()
        {
            // int ignoreRaycastLayer = LayerMask.NameToLayer("Player");
            // int layerMask = 1 << ignoreRaycastLayer;
            
            RaycastHit hit;
            bool isHitted = Physics.Raycast(
                _controller.GetCamera().gameObject.transform.position + _controller.GetCamera().gameObject.transform.forward * 1f,
                _controller.GetCamera().gameObject.transform.forward,
                out hit,
                1000f,
                1
            );

            bool isHitRemotePlayer = isHitted && hit.collider.gameObject.name.Contains("EnemyPlayer");
            
            NetworkManager.Instance.TryShoot(
                isHitted ? hit.transform.position : Vector3.zero, 
                isHitRemotePlayer,
                isHitRemotePlayer ? hit.collider.gameObject.gameObject.GetComponentInChildren<RemotePlayerComponent>().playerModel : null
            );
            
            if (isHitRemotePlayer)
                Debug.Log($"Hitted enemy! {hit.collider.gameObject.name}");
        }

        private void ReloadWeapon()
        {
            NetworkManager.Instance.TryReloadWeapon();
        }

        public void SwitchWeapon(byte slotId)
        {
            NetworkManager.Instance.TrySwitchWeapon(slotId);
        }

        public void UpdateByModel(UpdateLocalPlayerInfo model)
        {
            currentWeapon = model.CurrentWeapon;
            _weaponPoint.SetActiveWeapon(currentWeapon);
        }
    }
}