using System;
using System.Collections.Generic;
using System.Linq;

namespace WiseLabs.Conquest
{
    public class RoomManager
    {
        int _nextRoomId = 1;
        public static RoomManager Instance = new RoomManager();
        List<Room> Rooms { get; set; }
        public List<Player> Players { get; set; }

        private RoomManager()
        {
            Rooms = new List<Room>();
            Players = new List<Player>();
        }

        public Room JoinRoom(string connectionId)
        {
            var newPlayer = new Player()
            {
                ConnectionId = connectionId
            };

            foreach (var room in Rooms)
            {
                if (room.Players.Count == Constants.MaximumPlayerCount || room.StartTime.HasValue)
                { 
                    // Full or has already started
                    continue;
                }
                
                int? availablePlayerId = null;
                for (int i = 1; i <= Constants.MaximumPlayerCount; i++)
                {
                    if (room.Players.All(x => x.PlayerId != i))
                    {
                        availablePlayerId = i;
                        break;
                    }
                }
                if (!availablePlayerId.HasValue)
                {
                    throw new InvalidOperationException("availablePlayerId should have a value.");
                }
                newPlayer.PlayerId = availablePlayerId.Value;
                
                room.Players.Add(newPlayer);
                Players.Add(newPlayer);
                newPlayer.Room = room;
                return room;
            }

            // We need a new room!
            var newRoom = new Room();
            Rooms.Add(newRoom);

            newPlayer.PlayerId = 1;
            newRoom.Players.Add(newPlayer);
            Players.Add(newPlayer);
            newPlayer.Room = newRoom;
            return newRoom;                 
        }

        public void LeaveRoom(string connectionId, out int? playerId)
        {
            playerId = null;
            Room roomToDelete = null;
            foreach (var room in Rooms)
            {
                var player = room.Players.SingleOrDefault(x => x.ConnectionId == connectionId);
                if (player == null)
                {
                    continue;
                }

                playerId = player.PlayerId;
                
                room.Players.Remove(player);
                Players.Remove(player);

                if (room.Players.Count == 0)
                {
                    roomToDelete = room;
                    break;
                }
            }
            if (roomToDelete != null)
            {
                Rooms.Remove(roomToDelete);
            }
        }
    }
}