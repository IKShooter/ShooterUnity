using System.Collections.Generic;
using Network.Models;

namespace Events
{
    public delegate void RoomsLoadedEvent(List<RoomModel> rooms);
}