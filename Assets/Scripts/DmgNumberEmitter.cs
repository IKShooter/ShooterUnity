using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgNumberEmitter : MonoBehaviour
{
    public void SpawnNumber(int value)
    {
        GameObject asset = Resources.Load<GameObject>("Prefabs/DamageNumber");
        GameObject number = Instantiate(asset, transform.position, transform.rotation);
        number.GetComponent<DmgNumber>().StartAnim(value);
    }
}
