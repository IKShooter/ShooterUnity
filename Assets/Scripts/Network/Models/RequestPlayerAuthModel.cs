using LiteNetLib.Utils;

namespace Network.Models
{
    public class RequestPlayerAuthModel : INetSerializable
    {
        public string Nickname;
        public ushort VersionCode;

        public void Deserialize(NetDataReader reader)
        {
            Nickname = reader.GetString();
            VersionCode = reader.GetUShort();
        }

        public void Serialize(NetDataWriter writer)
        {
           writer.Put(Nickname);
           writer.Put(VersionCode);
        }
    }
}