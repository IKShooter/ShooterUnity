using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEmitter : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;
    private AudioClip shootClip;

    private float lifeTime = 0.5f;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Stop();

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 0.5f;
        shootClip = Resources.Load<AudioClip>("Sounds/PistolShoot");
    }

    public void Emit()
    {
        _audioSource.PlayOneShot(shootClip);
        StartCoroutine(StartEmit());
    }
    
    private IEnumerator StartEmit()
    {
        _particleSystem.Play();
        yield return new WaitForSeconds(lifeTime);
        _particleSystem.Stop();
    }
}