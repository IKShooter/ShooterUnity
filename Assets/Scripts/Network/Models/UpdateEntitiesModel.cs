using System.Collections.Generic;
using LiteNetLib.Utils;

namespace Network.Models
{
    public class UpdateEntitiesModel : INetSerializable
    {
        public List<EntityModel> Entities;

        public void Deserialize(NetDataReader reader)
        {
            Entities = new List<EntityModel>();
            
            int count = reader.GetInt();
            for (int i = 0; i < count; i++)
            {
                EntityModel entity = new EntityModel();
                entity.Deserialize(reader);
                Entities.Add(entity);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Entities.Count);
            foreach (EntityModel entity in Entities)
            {
                entity.Serialize(writer);
            }
        }
    }
}