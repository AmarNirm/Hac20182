using RoboCup.Entities;
using System;
namespace RoboCup
{
    /// <summary>
    /// SW -A sweeper is located at the back of the defensive line, just in front of the keeper.
    ///  This position is no longer popularly used in the modern game but was popular in the past especially with the catenaccio system of Italy. 
    /// A sweeper’s task is to clear the ball in case it breaks through the defenders. 
    /// He does not man-mark an opponent so he can collect loose balls or go upfield to bring the ball forward in attack.
    /// </summary>
    public class Sweeper : Defender
    {
        public Sweeper(Team team, ICoach coach)
            : base(team, coach)
        {
        }
    }
}