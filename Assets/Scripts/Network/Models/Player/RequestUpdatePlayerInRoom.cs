using LiteNetLib.Utils;
using UnityEngine;

namespace Network.Models.Player
{
    public class RequestUpdatePlayerInRoom : INetSerializable
    {
        public Vector3 Position;
        public float RotationY;

        public float RotationCameraX;

        public void Deserialize(NetDataReader reader)
        {
            Position = new Vector3();
            Position.x = reader.GetFloat();
            Position.y = reader.GetFloat();
            Position.z = reader.GetFloat();
            RotationY = reader.GetFloat();

            RotationCameraX = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Position.x);
            writer.Put(Position.y);
            writer.Put(Position.z);
            writer.Put(RotationY);

            writer.Put(RotationCameraX);
        }
    }
}