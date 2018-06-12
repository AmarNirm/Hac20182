using RoboCup.Entities;
using System;
namespace RoboCup
{
    /// <summary>
    /// A defender’s task is to keep the ball away from the keeper, 
    /// prevent opposing attackers from passing or receiving, 
    /// and block shots. Defending requires a player to be well-fit, 
    /// hard-working, and quick at anticipating the movement of the opponents.
    /// Defenders must protect the keeper: 
    /// they should think of the goalie as an important person that opponents are not allowed to get near to. Typically, teams play with four defenders.
    /// </summary>
    public class Defender : Player
    {
        public Defender(Team team, ICoach coach)
            : base(team, coach)
        {
           
        }
    }
}
