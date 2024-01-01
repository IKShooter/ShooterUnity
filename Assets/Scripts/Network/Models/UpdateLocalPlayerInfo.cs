using LiteNetLib.Utils;
using Network.Models;

namespace Server.Models
{
    public class UpdateLocalPlayerInfo : INetSerializable
    {
        //id надо вроде
        public byte Health;
        public bool IsDead;
        public LocalPlayerWeaponModel CurrentWeapon;

        public void Deserialize(NetDataReader reader)
        {
            Health = reader.GetByte();
            IsDead = reader.GetBool();
            CurrentWeapon = new LocalPlayerWeaponModel();
            CurrentWeapon.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Health);
            writer.Put(IsDead);
            CurrentWeapon.Serialize(writer);
        }
    }
}