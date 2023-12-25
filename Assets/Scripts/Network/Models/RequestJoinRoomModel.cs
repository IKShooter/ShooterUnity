using LiteNetLib.Utils;

namespace Server.Models
{
    public class RequestJoinRoomModel : INetSerializable
    {
        public string NameRoom;

        public void Deserialize(NetDataReader reader)
        {
            NameRoom = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NameRoom);
        }
    }
}