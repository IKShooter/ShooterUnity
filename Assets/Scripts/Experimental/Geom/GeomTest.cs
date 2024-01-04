using System;
using Player;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Experimental.Geom
{
    public class GeomTest : MonoBehaviour
    {
        [SerializeField] private GameObject geometryBox;
        private void Update()
        {
            if(!Input.GetKeyDown(KeyCode.E)) return;
            
            Ray ray = new Ray(
                new Vector3(PlayerController.Instance.GetMainCamera().transform.position.x, PlayerController.Instance.GetMainCamera().transform.position.y, PlayerController.Instance.GetMainCamera().transform.position.z), 
                new Vector3(PlayerController.Instance.GetMainCamera().transform.forward.x, PlayerController.Instance.GetMainCamera().transform.forward.y, PlayerController.Instance.GetMainCamera().transform.forward.z), 400f);
            AABB aabb = new AABB(
                new Vector3(geometryBox.transform.position.x,geometryBox.transform.position.y, geometryBox.transform.position.z),
                new Vector3(geometryBox.transform.position.x,geometryBox.transform.position.y, geometryBox.transform.position.z) + Vector3.One
            );
            bool isIntersect = Intersection.Intersects(ray, aabb);
            
            if(isIntersect)
                Debug.Log("!isIntersect!");
        }
    }
}