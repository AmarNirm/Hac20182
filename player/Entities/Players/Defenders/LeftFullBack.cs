using RoboCup.Entities;
using System;
namespace RoboCup
{
    /// <summary>
    /// (LB) A full-back is a defender positioned on the side.
    /// The defenders positioned between them are called centerbacks.
    /// The full-back is tasked to prevent opponents from attacking on the sides. He must be quick and must be able to prevent opponents from making a cross. He is often assigned to mark the opposing winger
    /// </summary>
    public class LeftFullBack : FullBack
    {
        public LeftFullBack(Team team, ICoach coach)
            : base(team, coach)
        {
        }
    }
}