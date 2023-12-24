using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayer : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }

    private static EnemyPlayer instance;
    public static EnemyPlayer Instance;

    public Player Player;

    void Start()
    {
        Debug.Log($"Start EnemyPlayer {Player.NickName}");
    }


    void Update()
    {
        if(instance == null) instance = this;
    }

    public void MovePlayer(Vector3 pos, float rotationY)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}
