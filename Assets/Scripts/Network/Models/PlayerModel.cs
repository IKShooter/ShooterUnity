using LiteNetLib.Utils;

namespace Network.Models
{
    public class PlayerModel : INetSerializable
    {
        public string Nickname;
        public int Id;

        public void Deserialize(NetDataReader reader)
        {
            Nickname = reader.GetString();
            Id = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Nickname);
            writer.Put(Id);
        }
    }
}