using Network.Models;
using UnityEngine;

public class WeaponPoint : MonoBehaviour
{
    private Weapon activeWeapon;
    private GameObject activeWeaponObject;
        
    public void RemoveActiveWeapon()
    {
        Destroy(activeWeaponObject);
        activeWeaponObject = null;
    }

    public void SetActiveWeapon(LocalPlayerWeaponModel weaponModel)
    {
        activeWeaponObject = Instantiate(Resources.Load<GameObject>($"Prefabs/Weapons/{weaponModel.Id}"));
        activeWeapon = activeWeaponObject.AddComponent<Weapon>();

        activeWeapon.SetDataFromModel(weaponModel);
        
        activeWeaponObject.transform.SetParent(transform);
        activeWeaponObject.transform.localPosition = Vector3.zero;
    }
}