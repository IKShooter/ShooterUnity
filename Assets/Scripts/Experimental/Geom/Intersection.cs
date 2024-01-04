using System;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;


public class Intersection
{
    private static float step = 0.1f;
    
    private static bool CheckCollision(AABB aabb, Vector3 point)
    {
        Vector3 minPos = aabb.Min;
        Vector3 maxPos = aabb.Max;
        return point.X >= minPos.X && point.X <= maxPos.X
                                   && point.Y >= minPos.Y && point.Y <= maxPos.Y
                                   && point.Z >= minPos.Z && point.Z <= maxPos.Z;
    }
    
    public static bool Intersects(Ray r, AABB aabb)
    {
        Vector3 pos = r.Point;

        while (Vector3.Distance(r.Point, pos) < r.rayLimit)
        {
            pos += r.Dir * step;
            
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new UnityEngine.Vector3(pos.X, pos.Y, pos.Z);
            
            if(CheckCollision(aabb, pos))
            {
                return true;
            }
        }

        return false;
    }
}