using System.Collections;
using UnityEngine;

public class ShootEmitter : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;
    private AudioClip _shootClip;

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
        _particleSystem.Play();
    }
}