using Network;
using Network.Models;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    public short id;
    public bool isCanZoom;
    public byte indexInSlot;
    public byte weight;
    public WeaponType type;
    public float fireRate;
    public float reloadTime;

    public void SetDataFromLocalModel(LocalPlayerWeaponModel weaponModel)
    {
        id = weaponModel.Id;
        isCanZoom = weaponModel.IsCanZoom;
        indexInSlot = weaponModel.IndexInSlot;
        weight = weaponModel.Weight;
        type = weaponModel.Type;
        fireRate = weaponModel.FireRate;
        reloadTime = weaponModel.ReloadTime;
    }

    public void SetDataFromRemoteModel(RemotePlayerWeaponModel weaponModel)
    {
        id = weaponModel.Id;
        isCanZoom = false;
        indexInSlot = 0;
        weight = 0;
        type = weaponModel.Type;
        fireRate = weaponModel.FireRate;
        reloadTime = weaponModel.ReloadTime;
    }
}