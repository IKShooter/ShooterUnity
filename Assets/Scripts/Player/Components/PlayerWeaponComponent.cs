﻿using System;
using System.Collections;
using System.Collections.Generic;
using Network;
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

        private bool isShoot;

        public PlayerWeaponComponent(PlayerController controller)
        {
            _controller = controller;
            _weaponPoint = controller.GetCamera().gameObject.GetComponentInChildren<WeaponPoint>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isShoot = false;
                _weaponPoint.DoReload(() =>
                {
                    ReloadWeapon();
                    return false;
                });
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && currentWeapon.Ammo > 0)
            {
                if (!_weaponPoint.GetWeaponAnimIK().IsReload)
                {
                    isShoot = true;
                    _controller.StartCoroutine(StartShootCoroutine());
                }
            } else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isShoot = false;
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
            while (isShoot)
            {
                if (currentWeapon.Ammo <= 0)
                {
                    isShoot = false;
                    yield break;
                }
                
                _weaponPoint.DoShoot();
                Shoot();

                switch (currentWeapon.Type)
                {
                    case WeaponType.Minigun:
                    case WeaponType.AssaultRifle:
                        yield return new WaitForSeconds(1f / currentWeapon.FireRate);
                        break;
                    
                    default:
                        isShoot = false;
                        yield break;
                }
            }
        }
        
        private void Shoot()
        {
            GameObject weaponGameObject = _weaponPoint.GetWeaponGameObject();
            weaponGameObject.GetComponentInChildren<ShootEmitter>()?.Emit();
            
            // int ignoreRaycastLayer = LayerMask.NameToLayer("Player");
            // int layerMask = 1 << ignoreRaycastLayer;

            bool isMelee = currentWeapon.Type == WeaponType.Melee;
            
            RaycastHit hit;
            bool isHitted = Physics.Raycast(
                _controller.GetCamera().gameObject.transform.position + _controller.GetCamera().gameObject.transform.forward * 1f,
                _controller.GetCamera().gameObject.transform.forward,
                out hit,
                isMelee ? 1f : 1000f,
                1
            );

            bool isHitRemotePlayer = isHitted && hit.collider.gameObject.name.Contains("EnemyPlayer");

            NetworkManager.Instance.TryShoot(
                isHitted ? hit.point : Vector3.zero, 
                isHitRemotePlayer,
                isHitRemotePlayer ? hit.collider.gameObject.gameObject.GetComponentInChildren<RemotePlayerComponent>().playerModel.Id : 0
            );
            
            if (isHitRemotePlayer)
                Debug.Log($"Hitted enemy! {hit.collider.gameObject.name}");

            if (currentWeapon.Type == WeaponType.Shotgun)
            {
                // TODO
                MakeShootImpulse();
            }
        }

        private void MakeShootImpulse()
        {
            CharacterController characterController = _controller.GetCharacterController();
            Vector3 impulseDirection = -_controller.transform.forward * 1.8f; // Adjust the impulse direction and magnitude as needed
            float impulseForce = 5.0f; // Adjust the force magnitude as needed

            // Apply the impulse using Move
            characterController.Move(impulseDirection * impulseForce * Time.deltaTime);
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