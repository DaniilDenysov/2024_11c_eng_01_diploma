using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DTOs
{
    [System.Serializable]
    public class Player
    {
        public string IP;
        public string Nickname;
        public string CharacterGUID;
        public int ConnectionId;
        public bool IsReady, IsPartyOwner;

        public Player()
        {
            
        }

        public Player(Player player)
        {
            IP = player.IP;
            Nickname = player.Nickname;
            CharacterGUID = player.CharacterGUID;
            ConnectionId = player.ConnectionId;
            IsReady = player.IsReady;
            IsPartyOwner = player.IsPartyOwner;
        }
    }
}
