using System;
using System.Collections.Generic;

namespace WiseLabs.Conquest
{
    public class Room
    {
        public Room()
        {
            Players = new List<Player>();
            var rand = new Random();
            MapId = rand.Next(1, 4);
        }

        public List<Player> Players { get; set; }
        public int MapId { get; set; }
        public long? StartTime { get; set; }
    }
}