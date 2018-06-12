using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    public class EnglandFormation : IFormation
    {
        public List<Player> InitTeam(Team team, ICoach coach)
        {
            var players = new List<Player>();
            players.Add(new NirAttacker(team, coach));
            return players;
        }
    }
}
