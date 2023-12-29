using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 1f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1f);
    }
}
