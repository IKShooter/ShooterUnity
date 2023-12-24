using LiteNetLib.Utils;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }

    private static PlayerNetwork instance;
    public static PlayerNetwork Instance => instance;

    public Player Player;

    private float timer;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(this != null) RoomLogic.Instance.playerNetwork = this;

        if(instance == null) instance = this;

        if(!Player.IsDeath)
        {
            timer += Time.fixedDeltaTime;

            if (timer >= NetworkManager.intervalUpdatePos)
            {
                Debug.Log($"Вызов кода {NetworkManager.intervalUpdatePos * 100f} раз в секунду");
                RequestUpdatePos();
                timer = 0f;
            }
        }
    }

    public struct ActionMessage
    {
        public ActionCode ActionCode { get; set; }
        public string Data { get; set; }
    }

    private void Spawn()
    {
        Vector3 pos = new Vector3(-2.259f, 1, -4.610f); // x y z

        PlayerData playerData = new PlayerData
        {
            PosX = pos.x,
            PosY = pos.y,
            PosZ = pos.z,
            RotationY = 0f
        };

        string sendData = JsonConvert.SerializeObject(playerData);

        SendData(sendData, ActionCode.SpawnPlayer);
    }

    public void RequestUpdatePos()
    {
        PlayerData playerData = new PlayerData
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            //RotationY = transform.rotation.y
            RotationY = transform.rotation.eulerAngles.y
        };

        Debug.Log($"Y: {playerData.RotationY}");

        string data = JsonConvert.SerializeObject(playerData);

        SendData(data, ActionCode.UpdatePosition);
    }

    public void SendData(string data, ActionCode actionCode)
    {
        NetDataWriter writer = new NetDataWriter();

        ActionMessage actionMessage = new ActionMessage
        {
            ActionCode = actionCode,
            Data = data
        };

        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);

        ClientConnect.client.FirstPeer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
    }
}

public class PlayerData
{
    public float PosX;
    public float PosY;
    public float PosZ;

    public float RotationY;
}