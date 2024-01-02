using System;
using UnityEngine;

namespace Experimental
{
    public class SinMovePlatform : MonoBehaviour
    {
        private float speed = 0.3f;
        private float amplitude = 12.0f;
        private float startX = 0.0f;

        private void Start()
        {
            startX = transform.position.x;
        }

        private void Update()
        {
            float xPosition = Mathf.Sin(Time.time * speed) * amplitude + startX;
            transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
        }
    }
}