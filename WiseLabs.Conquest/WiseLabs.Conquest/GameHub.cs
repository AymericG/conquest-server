using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WiseLabs.Conquest
{
    public static class Constants
    {
        public const int MinimumRequiredPlayerCountToPlay = 2;
        public const int MaximumPlayerCount = 4;
    }

    public class GameHub : Hub
    {

        public override Task OnConnected()
        {
            // Add user to a room that have space available
            var room = RoomManager.Instance.JoinRoom(Context.ConnectionId);
            var player = room.Players.Single(x => x.ConnectionId == Context.ConnectionId);
            
            if (room.Players.Count >= Constants.MinimumRequiredPlayerCountToPlay)
            {
                room.StartTime = DateTime.UtcNow.AddSeconds(2).ToUnixTime();
                Clients.AllExcept(Context.ConnectionId).onGameStart(room.StartTime.Value, room.MapId, room.Players.Select(x => x.PlayerId), null);
                Clients.Client(Context.ConnectionId).onGameStart(room.StartTime.Value, room.MapId, room.Players.Select(x => x.PlayerId), player.PlayerId);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            int? playerId;
            RoomManager.Instance.LeaveRoom(Context.ConnectionId, out playerId);
            if (playerId.HasValue)
            {
                Clients.All.onPlayerLeft(playerId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public void OnMove(dynamic moveInfo)
        {
            Clients.AllExcept(Context.ConnectionId).onMove(moveInfo);
        }

        public void OnLatestGameState(dynamic gameState)
        {
            Clients.AllExcept(Context.ConnectionId).onLatestGameState(gameState);
        }

    }
}