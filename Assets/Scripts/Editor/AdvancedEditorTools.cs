using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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

        [Serializable]
        private class GeometryBox
        {
            public Vector3 pos;
            public Vector3 size;
            public Vector3 rot;

            public GeometryBox(Vector3 pos, Vector3 size, Vector3 rot)
            {
                this.pos = pos;
                this.size = size;
                this.rot = rot;
            }
        }

        [Serializable]
        private class GeometryInfo
        {
            public List<GeometryBox> geometryBoxes;

            public GeometryInfo(List<GeometryBox> geometryBoxes)
            {
                this.geometryBoxes = geometryBoxes;
            }
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

        [MenuItem("Advanced/DumpLevelGeometry")]
        public static void DumpLevelGeometry()
        {
            List<GeometryBox> boxes = new List<GeometryBox>();
            
            foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                DoGrabGeomRecursive(ref boxes, obj);
            }
            
            string json = JsonUtility.ToJson(new GeometryInfo(boxes), true);
            using(StreamWriter writetext = new StreamWriter("./levelGeometry.json"))
            {
                writetext.WriteLine(json);
            }
            //Debug.Log($"\n{json}\n");
        }

        private static void DoGrabGeomRecursive(ref List<GeometryBox> boxes, GameObject go)
        {
            BoxCollider collider = go.GetComponent<BoxCollider>();
            if (collider != null && !collider.isTrigger)
            {
                bool isEqualSides = go.transform.localScale.x + go.transform.localScale.y - go.transform.localScale.z ==
                                    go.transform.localScale.y;
                //  isEqualSides ? collider.bounds.size : go.transform.localScale
                boxes.Add(new GeometryBox(collider.bounds.center, collider.bounds.size, go.transform.rotation.eulerAngles));
            }

            foreach (Transform tr in go.transform)
            {
                DoGrabGeomRecursive(ref boxes, tr.gameObject);
            }
        }
    }
}