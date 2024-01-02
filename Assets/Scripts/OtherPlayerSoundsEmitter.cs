using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerSoundsEmitter : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _hitClip;
    
    public void EmitHit()
    {
        _audioSource.PlayOneShot(_hitClip);
    }
    
    public static OtherPlayerSoundsEmitter CreateAndSpawn(Vector3 pos)
    {
        GameObject asset = Resources.Load<GameObject>("Prefabs/OtherPlayerSoundPoint");
        GameObject number = Instantiate(asset, pos, new Quaternion());
        return number.GetComponent<OtherPlayerSoundsEmitter>();
    }
}
