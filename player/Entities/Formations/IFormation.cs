using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    public interface IFormation
    {
        List<Player> InitTeam(Team team, ICoach coach);
    }
}
