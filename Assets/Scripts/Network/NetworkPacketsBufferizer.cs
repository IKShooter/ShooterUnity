using System;
using System.Collections.Generic;
using LiteNetLib.Utils;
using Network.Models.Player;
using UnityEngine;

namespace Network
{
    public class NetworkPacketsBufferizer
    {
        private const int OneTickPacketLimit = 12;
    
        private readonly NetworkManager _networkManager;
        private readonly Queue<Packet> _packetsQueue;
    
        public NetworkPacketsBufferizer(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _packetsQueue = new Queue<Packet>();
        }
    
        public void Add<T>(T packet, bool isHighPriority) where T : INetSerializable
        {
            _packetsQueue.Enqueue(new Packet(packet, HashCache<T>.Id, isHighPriority));
        }

        public int GetPacketsCount()
        {
            return _packetsQueue.Count;
        }

        public void Tick()
        {
            // It's maybe be a real problem!
            if (_packetsQueue.Count >= OneTickPacketLimit)
            {
                Debug.LogWarning($"Packets count are much! Have packets {_packetsQueue.Count} per max {OneTickPacketLimit}");
            }
        
            int packetsCount = Math.Min(_packetsQueue.Count, OneTickPacketLimit);
            for (int i = 0; i < packetsCount; i++)
            {
                Packet packet = _packetsQueue.Dequeue();
                // Debug.Log($"{i} : {packet}");
                bool isReliableOrdered = !(packet.NetSerializable is RequestUpdatePlayerInRoom);
                _networkManager.SendModel(ref packet, isReliableOrdered: isReliableOrdered);
            }
        }

        // TODO: use it in network "loop"
        public void OptimizeAllPackets()
        {
            // TODO: merge some packets
            // like a update pos packet
        }
    }
}