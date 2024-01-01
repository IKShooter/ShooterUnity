using LiteNetLib.Utils;
using Network.Models;

namespace Server.Models
{
    public class JoinedRoomModel : INetSerializable
    {
        public RoomModel Room;
        public PlayerModel PlayerModel;
        public void Deserialize(NetDataReader reader)
        {
            Room = new RoomModel();
            Room.Deserialize(reader);
            PlayerModel = new PlayerModel();
            PlayerModel.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Room);
            PlayerModel.Serialize(writer);
        }
    }
}