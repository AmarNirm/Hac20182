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
            players.Add(new Goalkeeper(team, coach));
            players.Add(new EnglandAttacker(team, coach));
            players.Add(new EnglandDefender(team, coach));
            return players;
        }
    }
}
