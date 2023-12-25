using LiteNetLib.Utils;

namespace Network.Models
{
    public class RequestPlayerAuthModel : INetSerializable
    {
        public string Nickname;

        public void Deserialize(NetDataReader reader)
        {
            Nickname = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
           writer.Put(Nickname);
        }
    }
}