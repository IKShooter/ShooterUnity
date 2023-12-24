using LiteNetLib.Utils;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

public class RoomLogic : MonoBehaviour
{
    private static RoomLogic instance;
    public static RoomLogic Instance => instance;

    public bool IsShowEscMenu = false;

    public GameObject EscMenu;

    public PlayerNetwork playerNetwork;

    public Spawn[] spawns;

    void Start()
    {
        InitPlayers();
        ChatManager.Instance.FillDropDownSelectPlayer();
    }

    void Update()
    {
        if(instance == null) instance = this;

        IsShowEscMenu = Input.GetKeyDown(KeyCode.Escape) ? !IsShowEscMenu : IsShowEscMenu;
        EscMenu.SetActive(IsShowEscMenu);

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Spawn();
        }
    }

    public struct ActionMessage
    {
        public ActionCode ActionCode { get; set; }
        public string Data { get; set; }
    }


    public PlayerData FindSpawnPoint()
    {
        spawns = FindObjectsOfType<Spawn>();

        int random = Random.Range(0, spawns.Length);

        Spawn spawn = spawns[random];

        PlayerData playerData = new PlayerData
        {
            PosX = spawn.transform.position.x,
            PosY = spawn.transform.position.y,
            PosZ = spawn.transform.position.z,
            RotationY = spawn.transform.rotation.eulerAngles.y,
        };

        return playerData;
    }

    public void DestroyPlayer(Player player)
    {
        EnemyPlayer[] players = FindObjectsOfType<EnemyPlayer>();
        EnemyPlayer pl = players.ToList().Find(p => p.Player.NickName == player.NickName);
        if (pl.transform.gameObject != null)
            Destroy(pl.transform.gameObject);
    }

    private void Spawn()
    {
        string sendData = JsonConvert.SerializeObject(FindSpawnPoint());

        SendData(sendData, ActionCode.SpawnPlayer);
    }

    public void MovePlayer(Player player)
    {
        EnemyPlayer[] players = FindObjectsOfType<EnemyPlayer>();
        EnemyPlayer pl = players.ToList().Find(p => p.Player.NickName == player.NickName);

        pl.MovePlayer(player.Position, player.rotationY);
    }

    public void SpawnLocalPlayer(string data)
    {
        Player player = JsonConvert.DeserializeObject<Player>(data);

        Vector3 position = player.Position;
        float rotationY = player.rotationY;

        GameObject G_EnemyPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/LocalPlayer"), position, new Quaternion(0f, rotationY, 0f, 0f));
        G_EnemyPlayer.GetComponent<PlayerNetwork>().Player = player;
        G_EnemyPlayer.name = player.NickName;
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

    public void SpawnPlayer(Player player)
    {
        Vector3 position = player.Position;
        float rotationY = player.rotationY;

        GameObject G_EnemyPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyPlayer"), position, new Quaternion(0f, rotationY, 0f, 0f));
        G_EnemyPlayer.GetComponent<EnemyPlayer>().Player = player;
        G_EnemyPlayer.name = player.NickName;

        Debug.Log($"Player: {G_EnemyPlayer.name} spawn");
    }

    public void InitPlayers()
    {
        foreach(Player player in ClientListener.CurrenRoom.Players)
        {
            if(!player.IsDeath)
            {
                SpawnPlayer(player);
            }
        }
    }
}