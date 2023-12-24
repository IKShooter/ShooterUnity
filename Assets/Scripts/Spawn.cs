using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float radius = 1f;
    public float distanceLine = 1f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * distanceLine);
    }
}
