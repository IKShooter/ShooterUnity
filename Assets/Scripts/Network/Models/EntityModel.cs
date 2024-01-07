using LiteNetLib.Utils;
using Network.Enums;
using UnityEngine;

namespace Network.Models
{
    public class EntityModel : INetSerializable
    {
        public Vector3 Position;
        public int Id;
        public EntityType EntityType;
        public ProjectileType ProjectileType;
        public KitType KitType;

        public void Deserialize(NetDataReader reader)
        {
            Position = new Vector3();
            Position.x = reader.GetFloat();
            Position.y = reader.GetFloat();
            Position.z = reader.GetFloat();

            Id = reader.GetInt();
            EntityType = (EntityType)reader.GetByte();

            if (EntityType == EntityType.Kit)
                KitType = (KitType)reader.GetByte();
            else if (EntityType == EntityType.Projectile)
                ProjectileType = (ProjectileType)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Position.x);
            writer.Put(Position.y);
            writer.Put(Position.z);

            writer.Put(Id);
            writer.Put((byte)EntityType);

            if(EntityType == EntityType.Kit)
                writer.Put((byte)KitType);
            else if(EntityType == EntityType.Projectile)
                writer.Put((byte)ProjectileType);
        }
    }
}