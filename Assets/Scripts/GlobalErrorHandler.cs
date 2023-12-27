using System;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalErrorHandler : MonoBehaviour
{
    private void Start()
    {
        EventsManager<ErrorEvent>.Register((tag, error, isCritical) =>
        {
            Debug.Log($"ERROR from {tag}: {error}");
            if (isCritical)
            {
                // TODO: Separated disconnect screen
                ShowErrorDialog(tag, $"Critical error! Restart game for reconnect to server.\nReason: {error.Message}");
            }
            else
            {
                ShowErrorDialog(tag, error.Message);
            }
        });
        
        DontDestroyOnLoad(this);
    }

    private void ShowErrorDialog(string title, string message)
    {
        GameObject dialogAsset = Resources.Load<GameObject>("Prefabs/UI/ErrorDialog");
        GameObject uiGameObject = Instantiate(dialogAsset);
        
        uiGameObject.GetComponentInChildren<RectTransform>().localPosition = Camera.main.ScreenToViewportPoint(Vector3.one * 0.5f);

        Text titleText = uiGameObject.GetComponentsInChildren<Text>()[0];
        Text messageText = uiGameObject.GetComponentsInChildren<Text>()[1];

        titleText.text = title;
        messageText.text = message;

        // Find canvas
        GameObject canvasGameObject = null;
        foreach (var gameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (gameObject.name.Contains("Canvas"))
            {
                canvasGameObject = gameObject;
                break;
            }
        }

        if (canvasGameObject == null)
        {
            Debug.Log("Canvas is not found!");
        }
        
        uiGameObject.transform.SetParent(canvasGameObject.transform);
    }
}