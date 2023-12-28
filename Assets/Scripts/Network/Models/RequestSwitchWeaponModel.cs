using LiteNetLib.Utils;

namespace Network.Models
{
    public class RequestSwitchWeaponModel : INetSerializable
    {
        public short SlotId;

        public void Deserialize(NetDataReader reader)
        {
            SlotId = reader.GetShort();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SlotId);
        }
    }
}