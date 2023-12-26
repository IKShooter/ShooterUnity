using System.Collections.Generic;
using Network.Models;

namespace Player
{
    public class RemotePlayersController
    {
        public RemotePlayersController Instance;

        public RemotePlayersController()
        {
            Instance = this;
        }

        private List<PlayerModel> _playerModels = new List<PlayerModel>();

        public void Update()
        {
            
        }
    }
}