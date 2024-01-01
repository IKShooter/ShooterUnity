using LiteNetLib;

namespace Events
{
    public delegate void ServerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo);
}