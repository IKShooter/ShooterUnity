using System;
using DitzelGames.FastIK;
using Experimental;
using JetBrains.Annotations;
using Network.Models;
using UnityEngine;

public class WeaponPoint : MonoBehaviour
{
    private Weapon _activeWeapon;
    private GameObject _activeWeaponObject;
    [CanBeNull] private WeaponAnimIK _weaponAnimIK;

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
        
    private void RemoveActiveWeapon()
    {
        Destroy(_activeWeaponObject);
        _activeWeaponObject = null;
        
        leftHand.GetComponent<FastIKFabric>().Target = null;
        rightHand.GetComponent<FastIKFabric>().Target = null;
    }

    public void SetActiveWeapon(RemotePlayerWeaponModel weaponModel)
    {
        if(_activeWeapon && _activeWeapon.id == weaponModel.Id)
            return;
        
        RemoveActiveWeapon();
        
        Destroy(_activeWeaponObject);

        _activeWeaponObject = Instantiate(Resources.Load<GameObject>($"Prefabs/Weapons/{weaponModel.Id}"));
        _activeWeapon = _activeWeaponObject.AddComponent<Weapon>();

        _activeWeapon.SetDataFromRemoteModel(weaponModel);
        
        UpdateWeapon(false);
    }

    public void SetActiveWeapon(LocalPlayerWeaponModel weaponModel)
    {
        if (_activeWeapon && _activeWeapon.id == weaponModel.Id)
            return;
        
        RemoveActiveWeapon();
        
        Destroy(_activeWeaponObject);

        _activeWeaponObject = Instantiate(Resources.Load<GameObject>($"Prefabs/Weapons/{weaponModel.Id}"));
        _activeWeapon = _activeWeaponObject.AddComponent<Weapon>();

        _activeWeapon.SetDataFromLocalModel(weaponModel);

        UpdateWeapon(true);
    }
    
    private void UpdateWeapon(bool isSecondLayer) 
    {
        _activeWeaponObject.transform.SetParent(transform, false);
        _activeWeaponObject.transform.localPosition = Vector3.zero;
        _activeWeaponObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        Transform leftHandTarget = null;
        Transform rightHandTarget = null;
        Transform pinPoint = null;
        foreach (Transform tr in _activeWeaponObject.transform)
        {
            if(!isSecondLayer)
                tr.gameObject.layer = LayerMask.NameToLayer("Default");
            
            if (tr.gameObject.name == "TargetLeftHand")
                leftHandTarget = tr;
            if (tr.gameObject.name == "TargetRightHand")
                rightHandTarget = tr;
            if (tr.gameObject.name == "PinPoint")
                pinPoint = tr;
        }
        
        leftHand.GetComponent<FastIKFabric>().Target = leftHandTarget;
        rightHand.GetComponent<FastIKFabric>().Target = rightHandTarget;

        if (pinPoint != null)
        {
            pinPoint.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            // Math new weapon position
            if (pinPoint != null)
            {
                var position = pinPoint.position;
                float aX = position.x;
                float aY = position.y;
                float aZ = position.z;

                var transform1 = transform;
                var position1 = transform1.position;
                float bX = position1.x;
                float bY = position1.y;
                float bZ = position1.z;

                float xF = (aX - bX);
                float yF = (aY - bY);
                float zF = (bZ - aZ);

                pinPoint.transform.localPosition = new Vector3(xF, yF, zF);
            }
        }

        // Add weapon anim to this weapon point
        _weaponAnimIK = gameObject.GetComponent<WeaponAnimIK>();
        if(_weaponAnimIK == null)
            _weaponAnimIK = gameObject.AddComponent<WeaponAnimIK>();
    }

    public GameObject GetWeaponGameObject()
    {
        return _activeWeaponObject;
    }

    public void DoShoot()
    {
        _weaponAnimIK?.DoShoot();
    }

    public void DoReload(Func<bool> callback)
    {
        _weaponAnimIK?.DoReload(_activeWeapon.reloadTime, callback);
    }

    public void UpdateIsMove(bool isMoving)
    {
        _weaponAnimIK?.UpdateIsMove(isMoving);
    }

    public WeaponAnimIK GetWeaponAnimIK()
    {
        return _weaponAnimIK;
    }
}