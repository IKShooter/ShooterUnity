using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    public string levelName;
    public GameObject spawnPoint;

    private string _lastLevelName = null;
    private Vector3 _lastPosition;

    public ScenesManager()
    {
        Instance = this;
    }
    
    private string GetLevelPath(string name)
    {
        return "Scenes/Maps/" + name;
    }

    public void LoadLevel(string name)
    {
        levelName = name;

        if (_lastLevelName != null && SceneManager.GetSceneByName(_lastLevelName).isLoaded)
            SceneManager.UnloadScene(_lastLevelName);
        
        SceneManager.LoadScene(GetLevelPath(name), LoadSceneMode.Additive);

        StartCoroutine(TeleportPlayerToSpawnPoint());
    }

    private IEnumerator TeleportPlayerToSpawnPoint()
    {
        Scene levelScene = SceneManager.GetSceneByName(GetLevelPath(levelName));
        while (!levelScene.isLoaded)
        {
            yield return null;
        }

        // Find spawn points and get one random
        List<GameObject> playersSpawns = levelScene.GetRootGameObjects().ToList().FindAll(obj => obj.name.Contains("PlayerSpawn"));
        spawnPoint = playersSpawns[Random.Range(0, playersSpawns.Count - 1)];

        if (spawnPoint == null)
            throw new Exception("Level does not have a spawn point!");

        CharacterController characterController = PlayerController.Instance.GetComponent<CharacterController>();
        characterController.enabled = false;
        PlayerController.Instance.gameObject.transform.position = spawnPoint.transform.position;
        characterController.enabled = true;
    }
}
