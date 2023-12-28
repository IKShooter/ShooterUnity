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
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
                SwitchWeapon(0);
        }
        
        public void SwitchWeapon(byte slotId)
        {
            NetworkManager.Instance.TrySwitchWeapon(slotId);
        }

        public void UpdateByModel(UpdateLocalPlayerInfo model)
        {
            currentWeapon = model.CurrentWeapon;
            _weaponPoint.RemoveActiveWeapon();
            _weaponPoint.SetActiveWeapon(currentWeapon);
        }
    }
}