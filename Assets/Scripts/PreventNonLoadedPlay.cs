dsusing System;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreventNonLoadedPlay : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Instance == null)
        {
            SceneManager.LoadScene("Scenes/Auth");
        }
    }
}