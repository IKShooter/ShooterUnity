using System;
using Events;
using UnityEngine;

public class GlobalErrorHandler : MonoBehaviour
{
    private void Start()
    {
        EventsManager<ErrorEvent>.Register((error, isCritical) =>
        {
            Debug.Log($"SERVER ERROR: {error}");
            if (isCritical)
            {
                // TODO
            }
        });
        
        DontDestroyOnLoad(this);
    }
}