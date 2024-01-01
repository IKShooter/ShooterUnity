using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = System.Numerics.Quaternion;

namespace Editor
{
    public class AdvancedEditorTools : EditorWindow
    {
        [Serializable]
        private class SpawnPoint
        {
            public float posX;
            public float posY;
            public float posZ;
            
            public float rotX;
            public float rotY;
            public float rotZ;

            public SpawnPoint(Vector3 pos, Vector3 rot)
            {
                posX = pos.x;
                posY = pos.y;
                posZ = pos.z;

                rotX = rot.x;
                rotY = rot.y;
                rotZ = rot.z;
            }
        }

        [Serializable]
        private class SpawnPointsList
        {
            public List<SpawnPoint> spawnPoints;
        }
        
        [MenuItem("Advanced/DumpSpawnPointsJson")]
        public static void DumpSpawnPointsJson()
        {
            SpawnPointsList spawnPointsList = new SpawnPointsList();
            spawnPointsList.spawnPoints = new List<SpawnPoint>();
            
            foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if(obj.name.Contains("PlayerSpawn"))
                    spawnPointsList.spawnPoints.Add(new SpawnPoint(obj.transform.position, obj.transform.rotation.eulerAngles));
            }
            
            string json = JsonUtility.ToJson(spawnPointsList, true);
            Debug.Log($"\n{json}\n");
        }       
    }
}