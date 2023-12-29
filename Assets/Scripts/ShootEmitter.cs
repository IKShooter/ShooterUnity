using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEmitter : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private float lifeTime = 0.5f;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Stop();
    }

    public void Emit()
    {
        StartCoroutine(StartEmit());
    }
    
    private IEnumerator StartEmit()
    {
        _particleSystem.Play();
        yield return new WaitForSeconds(lifeTime);
        _particleSystem.Stop();
    }
}