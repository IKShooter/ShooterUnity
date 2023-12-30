using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

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

        private bool isReload;

        public bool IsReload
        {
            get => isReload;
        }

        void Start()
        {
            originalPos = transform.localPosition;
        }

        void Update()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * interpolationSpeed);
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
            if(isFiring || isReload) yield break;
            
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
            
            targetPos = originalPos;
        }

        IEnumerator StartReload(float reloadTime, Func<bool> callback)
        {
            isReload = true;
            targetPos += -transform.up * 0.4f + transform.right * 0.1f;
            yield return new WaitForSeconds(reloadTime);
            targetPos = originalPos;
            isReload = false;

            callback.Invoke();
        }

        public void UpdateIsMove(bool isMove)
        {
            isMoving = isMove;
            if(isMoving)
                StartCoroutine(ContinuousMoveShake());
        }

        public void DoReload(float reloadTime, Func<bool> callback)
        {
            StartCoroutine(StartReload(reloadTime, callback));
        }

        public void DoShoot()
        {
            StartCoroutine(ApplyShake());
        }
    }
}
