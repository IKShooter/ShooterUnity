using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Models
{
    public class ErrorResultModel : INetSerializable
    {
        public ErrorType ErrorType;
        public bool IsCritical;

        public void Deserialize(NetDataReader reader)
        {
            ErrorType = (ErrorType)reader.GetUShort();
            IsCritical = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((ushort) ErrorType);
            writer.Put(IsCritical);
        }
    }
}
