using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Network.Models.Player
{
    public class UpdatePlayersTickInRoom : INetSerializable
    {
        public List<UpdatePlayerInRoom> updates;
        
        public void Deserialize(NetDataReader reader)
        {
            updates = new List<UpdatePlayerInRoom>();

            int size = reader.GetInt();
            for(var i = 0; i < size; i++) {
                UpdatePlayerInRoom mod = new UpdatePlayerInRoom();
                mod.Deserialize(reader);
                updates.Add(mod);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(updates.Count);
            for(var i = 0; i < updates.Count; i++) {
                updates[i].Serialize(writer);
            }
        }
    }
}