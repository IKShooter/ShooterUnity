using LiteNetLib.Utils;
using UnityEngine;

namespace Network.Models
{
    public class LocalPlayerRespawnModel : INetSerializable
    {
        public Vector3 Pos;
        public Vector3 Rot;

        public void Deserialize(NetDataReader reader)
        {
            Pos = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Rot = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Pos.x);
            writer.Put(Pos.y);
            writer.Put(Pos.z);

            writer.Put(Rot.x);
            writer.Put(Rot.y);
            writer.Put(Rot.z);
        }
    }
}