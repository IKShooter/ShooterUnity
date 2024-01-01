using LiteNetLib.Utils;

namespace Network.Models
{
    public class RoomModel : INetSerializable
    {
        public string Name;
        public string SceneName;

        public int PlayerCount;
        public int PlayerMax;

        public GameMode GameMode;

        public void Deserialize(NetDataReader reader)
        {
            Name = reader.GetString();
            SceneName = reader.GetString();
            PlayerCount = reader.GetInt();
            PlayerMax = reader.GetInt();
            GameMode = (GameMode)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Name);
            writer.Put(SceneName);
            writer.Put(PlayerCount);
            writer.Put(PlayerMax);
            writer.Put((byte)GameMode);
        }
    }
}