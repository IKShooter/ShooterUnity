using UnityEngine;
using System.Collections;

namespace Experimental
{
    public class WeaponAnimIK : MonoBehaviour
    {
        public float shakeDuration = 0.1f; 
        public float shakeAmount = 0.1f; 
        public float interpolationSpeed = 15f;

        private Vector3 originalPos;
        private Vector3 targetPos;
        private bool isFiring;
        private bool isMoving;

        void Start()
        {
            originalPos = transform.localPosition;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isFiring = true;
                StartCoroutine(ContinuousShake());
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isFiring = false;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                isMoving = true;
                StartCoroutine(ContinuousMoveShake());
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                isMoving = false;
            }
            
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * interpolationSpeed);
        }

        IEnumerator ContinuousShake()
        {
            while (isFiring)
            {
                yield return StartCoroutine(ApplyShake());
                yield return new WaitForSeconds(shakeDuration);
            }
        }

        IEnumerator ApplyShake()
        {
            float elapsedTime = 0f;
            Vector3 localTargetPos = Vector3.zero;

            while (elapsedTime < shakeDuration)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;
                localTargetPos = originalPos + shakeOffset;
                targetPos = localTargetPos;
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            targetPos = originalPos;
        }
        
        float factorX = 0.02f;
        float factorY = 0.04f;
        private float speedFactor = 4.5f;
        private float forwardOffset = -0.1f;
        
        IEnumerator ContinuousMoveShake()
        {
            float elapsedTime = 0f;
            Vector3 localTargetPos = Vector3.zero;
            
            while (isMoving)
            {
                float x = Mathf.Sin(elapsedTime) * factorX;
                float y = -Mathf.Cos(elapsedTime) * factorY;
                
                Vector3 shakeOffset = transform.up * y + transform.right * x + transform.forward * forwardOffset;
                localTargetPos = originalPos + shakeOffset;
                targetPos = localTargetPos;
                elapsedTime += Time.deltaTime * speedFactor;

                yield return null;
            }
            
            this.targetPos = originalPos;
        }
    }
}
