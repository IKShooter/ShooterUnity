using LiteNetLib.Utils;

namespace Network.Models
{
    public class RemotePlayerWeaponModel : INetSerializable
    {
        public short Id;
        public WeaponType Type;
        public float FireRate;
        public float ReloadTime;

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetShort();
            Type = (WeaponType)reader.GetByte();
            FireRate = reader.GetFloat();
            ReloadTime = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put((byte)Type);
            writer.Put(FireRate);
            writer.Put(ReloadTime);
        }
    }
}