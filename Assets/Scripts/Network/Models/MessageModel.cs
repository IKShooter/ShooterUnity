﻿using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;

namespace Server.Models
{
    public class MessageModel : INetSerializable
    {
        public string SenderNickname;
        public string Text;
        public TypeMessage Type;

        public void Deserialize(NetDataReader reader)
        {
            SenderNickname = reader.GetString();
            Text = reader.GetString();
            Type = (TypeMessage)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SenderNickname);
            writer.Put(Text);
            writer.Put((byte)Type);
        }
    }
}