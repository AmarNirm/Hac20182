using RoboCup.Entities;
using System;
using System.Drawing;

namespace RoboCup
{
    /// <summary>
    /// (CB) 
    /// In a four-player defense, the center-backs are the two defenders in the middle. 
    /// They are erroneously called center-halves, because in an obsolete football formation called the 2-3-5, 
    /// the “3” players are designated with that name. As tactics evolved, the “3” dropped to “center-back” but still retained the name “center-half.
    /// </summary>
    public class CentralDefenderRight : CentralDefender
    {
        public CentralDefenderRight(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 30, 10);
        }
    }
}