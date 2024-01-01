using System;
using System.Net;
using System.Net.Sockets;
using Events;
using LiteNetLib;

namespace Network
{
    class NetworkEventsController : INetEventListener
    {
        private NetworkManager _networkManager;

        public NetworkEventsController(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public void OnPeerConnected(NetPeer peer)
            => EventsManager<ServerConnectedEvent>.Trigger.Invoke(peer);

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) 
            => EventsManager<ServerDisconnectedEvent>.Trigger.Invoke(peer, disconnectInfo);

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
            => EventsManager<ErrorEvent>.Trigger.Invoke("Network error", new Exception( $"{socketError}"), true);

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _networkManager.NetPacketProcessor.ReadAllPackets(reader, peer);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        public void OnConnectionRequest(ConnectionRequest request) { }
    }
}