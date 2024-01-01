using LiteNetLib.Utils;

namespace Network.Models
{
    public class RequestCreateRoomAndJoinModel : INetSerializable
    {
        public string NameRoom;
        public string SceneName;

        public byte MaxPlayers;
        public byte GameMode;

        public void Deserialize(NetDataReader reader)
        {
            NameRoom = reader.GetString();
            SceneName = reader.GetString();
            MaxPlayers = reader.GetByte();
            GameMode = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NameRoom);
            writer.Put(SceneName);
            writer.Put(MaxPlayers);
            writer.Put(GameMode);
        }
    }
}