using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class OneShot : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.7f;

    private void Start()
    {
        StartCoroutine(StartOneShot());
    }

    private IEnumerator StartOneShot()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}