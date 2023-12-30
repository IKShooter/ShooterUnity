using LiteNetLib.Utils;
using Network.Enums;

namespace Network.Models
{
    public class DamageInfoModel : INetSerializable
    {
        public int PlayerHitedId;
        public byte Damage;
        public DamageType DamageType;

        public void Deserialize(NetDataReader reader)
        {
            PlayerHitedId = reader.GetInt();
            Damage = reader.GetByte();
            DamageType = (DamageType)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerHitedId);
            writer.Put(Damage);
            writer.Put((byte)DamageType);
        }
    }
}