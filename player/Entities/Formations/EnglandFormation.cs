using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using player.Entities.Players;

namespace RoboCup
{
    public class EnglandFormation : IFormation
    {
        public List<Player> InitTeam(Team team, ICoach coach)
        {
            var players = new List<Player>();
            players.Add(new EnglandAttackerFront(team, coach));
            players.Add(new EnglandAttackerBack(team, coach));
            players.Add(new EnglandDefenderUp(team, coach));
            players.Add(new EnglandDefenderDown(team, coach));
            players.Add(new Goalkeeper(team, coach));
            return players;
        }
    }
}
