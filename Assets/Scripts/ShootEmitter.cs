using System.Collections;
using UnityEngine;

public class ShootEmitter : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;
    private AudioClip _shootClip;

    private const float LifeTime = 0.5f;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Stop();

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 0.5f;
        _shootClip = Resources.Load<AudioClip>("Sounds/PistolShoot");
    }

    public void Emit()
    {
        _audioSource.PlayOneShot(_shootClip);
        StartCoroutine(StartEmit());
    }
    
    private IEnumerator StartEmit()
    {
        _particleSystem.Play();
        yield return new WaitForSeconds(LifeTime);
        _particleSystem.Stop();
    }
}