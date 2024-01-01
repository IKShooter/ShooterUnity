using UnityEngine;

public class DmgNumberEmitter : MonoBehaviour
{
    public void SpawnNumber(int value, Color color)
    {
        GameObject asset = Resources.Load<GameObject>("Prefabs/DamageNumber");
        var transform1 = transform;
        GameObject number = Instantiate(asset, transform1.position, transform1.rotation);
        number.GetComponent<DmgNumber>().StartAnim(value, color);
    }
}
