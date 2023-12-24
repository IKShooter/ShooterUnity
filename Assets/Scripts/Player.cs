using UnityEngine;

using LiteNetLib.Utils;

public class Player : INetSerializable
{
    public string NickName;
    public int Id;

    public PlayerSceneState PlayerSceneState;
    public Room? CurrentRoom;
    public Permissions Permission;


    public bool IsDeath;

    public Vector3 Position;

    public float rotationY;

    public void Deserialize(NetDataReader reader)
    {
        NickName = reader.GetString();
        Id = reader.GetInt();

        PlayerSceneState = (PlayerSceneState)reader.GetByte();

        Room r = new Room();
        r.Deserialize(reader);
        CurrentRoom = r;
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NickName);
        writer.Put(Id);
        writer.Put((byte)PlayerSceneState);

        writer.Put(Position.x);
        writer.Put(Position.y);
        writer.Put(Position.z);

        writer.Put(rotationY);
    }
}