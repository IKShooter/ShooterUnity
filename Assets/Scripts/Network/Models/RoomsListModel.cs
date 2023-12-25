using System.Collections.Generic;
using LiteNetLib.Utils;
using Network.Models;

namespace Server.Models
{
    public class RoomsListModel : INetSerializable
    {
        public List<RoomModel> roomListModel = new List<RoomModel>();

        public void Deserialize(NetDataReader reader)
        {
            int countRoomc = reader.GetInt();

            for(int i = 0; i < countRoomc; i++)
            {
                RoomModel roomModel = new RoomModel();
                roomModel.Deserialize(reader);
                roomListModel.Add(roomModel);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(roomListModel.Count);
            foreach(RoomModel roomModel in roomListModel)
            {
                writer.Put(roomModel);
            }
        }
    }
}