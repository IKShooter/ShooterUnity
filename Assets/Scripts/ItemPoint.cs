using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPoint : MonoBehaviour
{
    public enum ItemType {
        AMMUNITION,
        HEALTHKIT
    }

    public ItemType itemType;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Handles.Label(transform.position, itemType.ToString());
    }
}
