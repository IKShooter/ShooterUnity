using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // Get the bounds of the mesh renderer
            Bounds bounds = meshRenderer.bounds;

            // Draw wireframe cube representing the bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            // Get the object's transform
            Transform transform = this.transform;

            // Draw X-axis line
            Vector3 right = transform.right * bounds.extents.x;
            Gizmos.DrawLine(bounds.center - right, bounds.center + right);

            // Draw Z-axis line
            Vector3 forward = transform.forward * bounds.extents.z;
            Gizmos.DrawLine(bounds.center - forward, bounds.center + forward);
        }
    }
}
