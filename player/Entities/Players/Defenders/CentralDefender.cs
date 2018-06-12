using RoboCup.Entities;
using System;
namespace RoboCup
{
    /// <summary>
    /// (CB) 
    /// In a four-player defense, the center-backs are the two defenders in the middle. 
    /// They are erroneously called center-halves, because in an obsolete football formation called the 2-3-5, 
    /// the “3” players are designated with that name. As tactics evolved, the “3” dropped to “center-back” but still retained the name “center-half.
    /// </summary>
    public class CentralDefender : Defender
    {
        public CentralDefender(Team team, ICoach coach)
            : base(team, coach)
        {
        }

        public override void play()
        {

            // first ,ove to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            ObjectInfo obj;

            while (!m_timeOver)
            {
            }
        }
    }
}