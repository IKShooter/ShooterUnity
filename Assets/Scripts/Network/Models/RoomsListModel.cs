using System.Collections.Generic;
using LiteNetLib.Utils;
using Network.Models;

namespace Server.Models
{
    public class RoomsListModel : INetSerializable
    {
        public List<RoomModel> roomListModel;

        public void Deserialize(NetDataReader reader)
        {
            roomListModel = new List<RoomModel>();
            
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
                roomModel.Serialize(writer);
            }
        }
    }
}