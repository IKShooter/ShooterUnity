using Network;
using Network.Models;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public short Id;
    public bool IsCanZoom;
    public byte IndexInSlot;
    public byte Weight;
    public WeaponType Type;
    public float FireRate;
    public float ReloadTime;

    public void SetDataFromLocalModel(LocalPlayerWeaponModel weaponModel)
    {
        Id = weaponModel.Id;
        IsCanZoom = weaponModel.IsCanZoom;
        IndexInSlot = weaponModel.IndexInSlot;
        Weight = weaponModel.Weight;
        Type = weaponModel.Type;
        FireRate = weaponModel.FireRate;
        ReloadTime = weaponModel.ReloadTime;
    }

    public void SetDataFromRemoteModel(RemotePlayerWeaponModel weaponModel)
    {
        Id = weaponModel.Id;
        IsCanZoom = false;
        IndexInSlot = 0;
        Weight = 0;
        Type = weaponModel.Type;
        FireRate = weaponModel.FireRate;
        ReloadTime = weaponModel.ReloadTime;
    }
}