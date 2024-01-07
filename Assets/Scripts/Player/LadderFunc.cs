using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class LadderFunc : MonoBehaviour
{
    private bool isPlayerIn;
    
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerController.Instance.gameObject == other.gameObject)
            isPlayerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (PlayerController.Instance.gameObject == other.gameObject)
            isPlayerIn = false;
    }

    private void Update()
    {
        if (isPlayerIn && Input.GetKey(KeyCode.W))
        {
            PlayerController.Instance.MovementComponent.GetMotor().LiftUp();
        }
    }
}
