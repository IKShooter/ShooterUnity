﻿using LiteNetLib.Utils;
using UnityEngine;

namespace Network.Models
{
    public class RequestShootModel : INetSerializable
    {
        public Vector3 PosTo;
        public bool IsHit;
        public int TargetPlayerId;

        public void Deserialize(NetDataReader reader)
        {
            PosTo = new Vector3();
            PosTo.x = reader.GetFloat();
            PosTo.y = reader.GetFloat();
            PosTo.z = reader.GetFloat();

            IsHit = reader.GetBool();
            if (IsHit)
            {
                TargetPlayerId = reader.GetInt();
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PosTo.x);
            writer.Put(PosTo.y);
            writer.Put(PosTo.z);

            writer.Put(IsHit);

            if(IsHit)
                writer.Put(TargetPlayerId);
        }
    }
}