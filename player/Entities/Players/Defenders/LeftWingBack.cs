using RoboCup.Entities;
using System;
namespace RoboCup
{
    /// <summary>
    ///LWB  A wing-back is a full-back that advances up to the opponent’s goal end. 
    /// He runs the whole length of the football pitch: 
    /// he defends the flanks like a dedicated full-back and attacks like a winger.This is the most physically demanding position on the field. 
    /// </summary>
    public class LeftWingBack : WingBack
    {
        public LeftWingBack(Team team, ICoach coach)
            : base(team, coach)
        {
        }
    }
}