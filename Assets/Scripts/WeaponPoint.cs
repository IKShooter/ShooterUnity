using System;
using DitzelGames.FastIK;
using Network.Models;
using UnityEngine;

public class WeaponPoint : MonoBehaviour
{
    private Weapon activeWeapon;
    private GameObject activeWeaponObject;

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
        
        UpdateWeapon();
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

        UpdateWeapon();
    }
    
    private void UpdateWeapon() 
    {
        activeWeaponObject.transform.SetParent(transform, false);
        activeWeaponObject.transform.localPosition = Vector3.zero;
        activeWeaponObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        Transform leftHandTarget = null;
        Transform rightHandTarget = null;
        Transform pinPoint = null;
        foreach (Transform tr in activeWeaponObject.transform)
        {
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
        float aX = pinPoint.localPosition.x;
        float aY = pinPoint.localPosition.y;
        float aZ = pinPoint.localPosition.z;

        float bX = transform.localPosition.x;
        float bY = transform.localPosition.y;
        float bZ = transform.localPosition.z;

        float xF = (aX - bX);
        float yF = (aY - bY);
        float zF = (bZ - aZ);

        activeWeaponObject.transform.localPosition = new Vector3(xF, yF, zF);
    }

    public GameObject GetWeaponGameObject()
    {
        return activeWeaponObject;
    }
}