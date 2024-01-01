using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        var position = transform.position;
        Gizmos.DrawWireCube(position, Vector3.one * 0.5f);
        Gizmos.DrawLine(position, position + transform.up * 1f);
        Gizmos.DrawLine(position, position + transform.forward * 1f);
    }
}
