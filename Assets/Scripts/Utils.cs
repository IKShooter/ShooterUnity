using UnityEngine;

public static class Utils
{
    public static void SpawnPrefab(string resPath, Vector3 pos, Quaternion rot)
    {
        GameObject asset = Resources.Load<GameObject>($"Prefabs/{resPath}");
        Object.Instantiate(asset, pos, rot);
    }
    
    public static void SpawnHitParticle(Vector3 pos)
    {
        SpawnPrefab("ShootHitParticles", pos, Quaternion.Euler(0f, 0f, 0f));
    }
}