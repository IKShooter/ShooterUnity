using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalErrorHandler : MonoBehaviour
{
    private void Start()
    {
        EventsManager<ErrorEvent>.Register((errorTag, error, isCritical) =>
        {
            Debug.Log($"ERROR from {errorTag}: {error}");
            // TODO: Separated disconnect screen
            ShowErrorDialog(errorTag,
                isCritical
                    ? $"Critical error! Restart game for reconnect to server.\nReason: {error.Message}"
                    : error.Message);
        });
        
        DontDestroyOnLoad(this);
    }

    private void ShowErrorDialog(string title, string message)
    {
        GameObject dialogAsset = Resources.Load<GameObject>("Prefabs/UI/ErrorDialog");
        GameObject uiGameObject = Instantiate(dialogAsset);

        Text titleText = uiGameObject.GetComponentsInChildren<Text>()[0];
        Text messageText = uiGameObject.GetComponentsInChildren<Text>()[1];

        titleText.text = title;
        messageText.text = message;

        // Find canvas
        GameObject canvasGameObject = null;
        foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (rootGameObject.name.Contains("Canvas"))
            {
                canvasGameObject = rootGameObject;
                break;
            }
        }

        if (canvasGameObject == null)
        {
            Debug.Log("Canvas is not found!");
        }

        uiGameObject.GetComponentsInChildren<Button>()[0].onClick.AddListener(() =>
        {
            Destroy(uiGameObject);
        });

        if (canvasGameObject != null)
        {
            uiGameObject.transform.SetParent(canvasGameObject.transform);

            // Magic centering
            RectTransform rectTransform = uiGameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasGameObject.transform, false);
            rectTransform.localPosition = Vector3.zero + new Vector3(0f, 0f, 1f);
        }
    }
}