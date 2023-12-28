using LiteNetLib.Utils;

namespace Network.Models
{
    public class LocalPlayerWeaponModel : INetSerializable
    {
        public short Id;
        public short Damage;
        public bool IsCanZoom;
        public byte IndexInSlot;
        public byte Weight;
        public WeaponType Type;
        public ushort Ammo;
        public ushort AmmoReserve;
        public float FireRate;
        public float ReloadTime;

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetShort();
            Damage = reader.GetShort();
            IsCanZoom = reader.GetBool();
            IndexInSlot = reader.GetByte();
            Weight = reader.GetByte();
            Type = (WeaponType)reader.GetByte();
            Ammo = reader.GetUShort();
            AmmoReserve = reader.GetUShort();
            FireRate = reader.GetFloat();
            ReloadTime = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Damage);
            writer.Put(IsCanZoom);
            writer.Put(IndexInSlot);
            writer.Put(Weight);
            writer.Put((byte)Type);
            writer.Put(Ammo);
            writer.Put(AmmoReserve);
            writer.Put(FireRate);
            writer.Put(ReloadTime);
        }
    }
}