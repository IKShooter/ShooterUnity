using System;
using DitzelGames.FastIK;
using Experimental;
using JetBrains.Annotations;
using Network.Models;
using UnityEngine;

public class WeaponPoint : MonoBehaviour
{
    private Weapon activeWeapon;
    private GameObject activeWeaponObject;
    [CanBeNull] private WeaponAnimIK _weaponAnimIK;

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
        
    private void RemoveActiveWeapon()
    {
        Destroy(activeWeaponObject);
        activeWeaponObject = null;
        
        leftHand.GetComponent<FastIKFabric>().Target = null;
        rightHand.GetComponent<FastIKFabric>().Target = null;
    }

    public void SetActiveWeapon(RemotePlayerWeaponModel weaponModel)
    {
        if(activeWeapon && activeWeapon.Id == weaponModel.Id)
            return;
        
        RemoveActiveWeapon();
        
        Destroy(activeWeaponObject);

        activeWeaponObject = Instantiate(Resources.Load<GameObject>($"Prefabs/Weapons/{weaponModel.Id}"));
        activeWeapon = activeWeaponObject.AddComponent<Weapon>();

        activeWeapon.SetDataFromRemoteModel(weaponModel);
        
        UpdateWeapon(false);
    }

    public void SetActiveWeapon(LocalPlayerWeaponModel weaponModel)
    {
        if (activeWeapon && activeWeapon.Id == weaponModel.Id)
            return;
        
        RemoveActiveWeapon();
        
        Destroy(activeWeaponObject);

        activeWeaponObject = Instantiate(Resources.Load<GameObject>($"Prefabs/Weapons/{weaponModel.Id}"));
        activeWeapon = activeWeaponObject.AddComponent<Weapon>();

        activeWeapon.SetDataFromLocalModel(weaponModel);

        UpdateWeapon(true);
    }
    
    private void UpdateWeapon(bool isSecondLayer) 
    {
        activeWeaponObject.transform.SetParent(transform, false);
        activeWeaponObject.transform.localPosition = Vector3.zero;
        activeWeaponObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        Transform leftHandTarget = null;
        Transform rightHandTarget = null;
        Transform pinPoint = null;
        foreach (Transform tr in activeWeaponObject.transform)
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
        
        pinPoint.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        // Math new weapon position
        float aX = pinPoint.position.x;
        float aY = pinPoint.position.y;
        float aZ = pinPoint.position.z;

        float bX = transform.position.x;
        float bY = transform.position.y;
        float bZ = transform.position.z;

        float xF = (aX - bX);
        float yF = (aY - bY);
        float zF = (bZ - aZ);

        activeWeaponObject.transform.localPosition = new Vector3(xF, yF, zF);
        
        // Add weapon anim to this weapon point
        _weaponAnimIK = gameObject.GetComponent<WeaponAnimIK>();
        if(_weaponAnimIK == null)
            _weaponAnimIK = gameObject.AddComponent<WeaponAnimIK>();
    }

    public GameObject GetWeaponGameObject()
    {
        return activeWeaponObject;
    }

    public void DoShoot()
    {
        _weaponAnimIK?.DoShoot();
    }

    public void DoReload(Func<bool> callback)
    {
        _weaponAnimIK?.DoReload(activeWeapon.ReloadTime, callback);
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