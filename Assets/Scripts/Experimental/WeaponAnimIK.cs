using UnityEngine;
using System.Collections;

namespace Experimental
{
    public class WeaponAnimIK : MonoBehaviour
    {
        public Transform weaponTransform; // Ссылка на трансформ оружия для тряски
        public float shakeDuration = 0.1f; // Длительность одного цикла тряски
        public float shakeAmount = 0.1f; // Интенсивность тряски
        public float interpolationSpeed = 15f; // Скорость интерполяции

        private Vector3 originalPos; // Исходная позиция оружия
        private bool isFiring = false; // Флаг для определения, происходит ли выстрел

        void Start()
        {
            if (weaponTransform != null)
            {
                originalPos = weaponTransform.localPosition;
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // ЛКМ нажата
            {
                isFiring = true;
                StartCoroutine(ContinuousShake());
            }
            else if (Input.GetMouseButtonUp(0)) // ЛКМ отпущена
            {
                isFiring = false;
            }
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
            Vector3 targetPos = Vector3.zero;

            while (elapsedTime < shakeDuration)
            {
                // Создание случайной тряски в заданных пределах
                Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;

                // Установка целевой позиции для интерполяции
                targetPos = originalPos + shakeOffset;

                // Интерполяция до целевой позиции
                weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, targetPos, Time.deltaTime * interpolationSpeed);

                // Увеличение времени прошедшего с начала тряски
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Возвращение оружия в исходную позицию после тряски
            weaponTransform.localPosition = originalPos;
        }
    }
}
