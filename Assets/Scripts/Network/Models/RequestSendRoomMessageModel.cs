using LiteNetLib.Utils;
using Network;

namespace Server.Models
{
    public class RequestSendRoomMessageModel : INetSerializable
    {
        public string Text;
        public TypeMessage Type;

        public void Deserialize(NetDataReader reader)
        {
            Text = reader.GetString();
            Type = (TypeMessage)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Text);
            writer.Put((byte)Type);
        }
    }
}