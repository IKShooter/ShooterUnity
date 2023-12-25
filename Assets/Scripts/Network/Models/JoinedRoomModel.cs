using LiteNetLib.Utils;
using Network.Models;

namespace Server.Models
{
    public class JoinedRoomModel : INetSerializable
    {
        public RoomModel Room;
        public void Deserialize(NetDataReader reader)
        {
            Room = new RoomModel();
            Room.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Room);
        }
    }
}