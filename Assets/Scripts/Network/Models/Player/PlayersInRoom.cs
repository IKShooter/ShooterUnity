using System.Collections.Generic;
using LiteNetLib.Utils;
using Network.Models;

namespace Network.Models.Player
{
    public class PlayersInRoom : INetSerializable
    {
        public List<PlayerModel> Players;

        public void Deserialize(NetDataReader reader)
        {
            var count = reader.GetInt();
            for (int i = 0; i < count; i++)
            {
                var player = new PlayerModel();
                player.Deserialize(reader);
                Players.Add(player);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            Players = new List<PlayerModel>();
            writer.Put(Players.Count);
            foreach (var player in Players)
            {
                player.Serialize(writer);
            }
        }
    }
}