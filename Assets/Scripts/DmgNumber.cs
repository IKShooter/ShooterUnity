using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Random = System.Random;

public class DmgNumber : MonoBehaviour
{
    private Vector3? moveTarget;

    private void Start()
    {
        UpdateRotation();
    }

    public void StartAnim(int value, Color color)
    {
        moveTarget = transform.position;

        GetComponent<TextMesh>().color = color;
        GetComponent<TextMesh>().text = value.ToString();
        StartCoroutine(AnimCoroutine());
    }

    private Random _random = new Random();
    
    public double GetRandomNumberInRange(double minNumber, double maxNumber)
    {
        return _random.NextDouble() * (maxNumber - minNumber) + minNumber;
    }
    
    private IEnumerator AnimCoroutine()
    {
        float offsetA = (float)GetRandomNumberInRange(-2.5, 2.5);
        
        moveTarget = transform.position + transform.up * 5.0f + transform.right * offsetA;
        yield return new WaitForSeconds(2f);
        moveTarget = transform.position + -transform.up * 3.5f + transform.right * offsetA;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void UpdateRotation()
    {
        // Autorotate to camera
        GameObject go = PlayerController.Instance.GetMainCamera().gameObject;
        Vector3 heading = go.transform.position - transform.position;
        transform.LookAt(transform.position - heading);
    }

    void Update()
    {
        UpdateRotation();
        
        if(moveTarget != null)
            transform.position = Vector3.Lerp(transform.position, moveTarget.Value, Time.deltaTime * 0.4f);
    }
}
