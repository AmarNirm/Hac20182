using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using RoboCup.Infrastructure;

namespace RoboCup
{
    public class EnglandDefenderUp : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;
        private bool Init;
        public EnglandDefenderUp(Team team, ICoach coach)
            : base(team, coach)
        {
            if(m_side=='l')
            {
                m_startPosition.X = -25;
                m_startPosition.Y = -10;
            }
            else
            {
                m_startPosition.X = 25;
                m_startPosition.Y = -10;
            }
        }

        public override void play()
        {
            // first move to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenCoachObject ball;
            PointF? goal;

            bool didIReachToGoal = false;

            while (!m_timeOver)
            {
                try
                {
                    ball = GetBall();
                    if ((m_side == 'l' && ball.Pos.Value.X <= GetCurrPlayer().Pos.Value.X) ||
                        (m_side == 'r' && ball.Pos.Value.X >= GetCurrPlayer().Pos.Value.X))
                    {
                        didIReachToGoal = false;
                    }

                    if (IsBallInMyHalf())
                    {
                        ball = GetBall();
                        if (GetDistanceFrom(ball.Pos.Value) > 1.5)
                        {
                            if (AmITheClosesDefenderToBall())
                            {
                                MoveToPosition(ball.Pos.Value, null);
                            }
                            else
                            {
                                if (!didIReachToGoal)
                                {
                                    var pointNearGoal = GetGoalPosition(true).Value;
                                    pointNearGoal.X += m_side == 'l' ? 6f : -6f;
                                    pointNearGoal.Y += 3f;
                                    didIReachToGoal = MoveToPosition(pointNearGoal, null); 
                                }
                                else
                                {
                                    MoveToPosition(GetBall().Pos.Value, null);
                                }
                            }
                        }
                        else
                        {
                            //if distance is too far for the ball to reach the attacker
                            if (AmIInMyHalf() && GetDistanceFrom(FindAttackerPosition()) > 28)
                                Kick(FindAttackerPosition(), 20);
                            else
                                Kick(FindAttackerPosition());
                        }
                    }
                    else
                    {
                        ball = GetBall();
                        var currPlayer = this.GetCurrPlayer();
                        var nextPos = new PointF(currPlayer.Pos.Value.X, ball.Pos.Value.Y);
                        MoveToPosition(nextPos, OpponentGoal);
                    }

                    // sleep one step to ensure that we will not send
                    // two commands in one cycle.
                    try
                    {
                        Thread.Sleep(2 * SoccerParams.simulator_step);
                    }
                    catch (Exception e)
                    {

                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("basa");
                }
            }
        }
        
        public bool IsBallInMyHalf()
        {
            var ball = GetBall();
            bool onDefenderSide = false;
            if (m_side == 'l')
                onDefenderSide = ball.Pos.Value.X <= 5;
            else
                onDefenderSide = ball.Pos.Value.X >= -5;

            return onDefenderSide;
        }


        public bool AmITheClosesDefenderToBall()
        {
            return FindDefenderClosestToTheBall() == this.m_number;
        }

    }
}
