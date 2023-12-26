using LiteNetLib.Utils;
using UnityEngine;

namespace Network.Models.Player
{
    public class UpdatePlayerInRoom : INetSerializable
    {
        public int Id;
        public Vector3 Position;
        public float RotationY;

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();

            Position = new Vector3();
            Position.x = reader.GetFloat();
            Position.y = reader.GetFloat();
            Position.z = reader.GetFloat();
            RotationY = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);

            writer.Put(Position.x);
            writer.Put(Position.y);
            writer.Put(Position.z);
            writer.Put(RotationY);
        }
    }
}