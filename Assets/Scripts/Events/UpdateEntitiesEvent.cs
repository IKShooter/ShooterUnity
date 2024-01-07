using System.Collections.Generic;
using Network.Models;

namespace Events
{
    public delegate void UpdateEntitiesEvent(List<EntityModel> entities);
}