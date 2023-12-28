using LiteNetLib.Utils;
using UnityEngine;

namespace Network.Models.Player
{
    public class UpdatePlayerInRoom : INetSerializable
    {
        public int Id;
        public ushort Ping;
        public bool IsDead;
        public Vector3 Position;
        public float RotationY;
        public RemotePlayerWeaponModel CurrentWeapon;
        
        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            Ping = reader.GetUShort();
            CurrentWeapon = new RemotePlayerWeaponModel();
            CurrentWeapon.Deserialize(reader);

            IsDead = reader.GetBool();

            Position = new Vector3();
            Position.x = reader.GetFloat();
            Position.y = reader.GetFloat();
            Position.z = reader.GetFloat();
            RotationY = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Ping);
            CurrentWeapon.Serialize(writer);
            writer.Put(IsDead);

            writer.Put(Position.x);
            writer.Put(Position.y);
            writer.Put(Position.z);
            writer.Put(RotationY);
        }
    }
}