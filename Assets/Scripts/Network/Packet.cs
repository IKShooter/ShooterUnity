using LiteNetLib.Utils;

namespace Network
{
    public class Packet
    {
        public readonly INetSerializable NetSerializable;
        public readonly ulong Hash;
        public readonly bool IsHighPriority;

        public Packet(INetSerializable netSerializable, ulong hash, bool isHighPriority)
        {
            NetSerializable = netSerializable;
            Hash = hash;
            IsHighPriority = isHighPriority;
        }
    }
}