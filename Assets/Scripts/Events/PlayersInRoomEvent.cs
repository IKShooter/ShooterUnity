using System.Collections.Generic;
using Network.Models;

namespace Events
{
    public delegate void PlayersInRoomEvent(List<PlayerModel> playersModels);
}