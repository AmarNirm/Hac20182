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


                    if (!Init)
                    {
                        Init = MoveToPosition(m_startPosition, OpponentGoal);
                        if (Init)
                        {
                            Console.WriteLine("Defender in position!");
                        }
                    }

                    else if (IsBallInMyHalf())
                    {
                        ball = GetBall();
                        if (GetDistanceFrom(ball.Pos.Value) > 1.5)
                        {
                            if (AmITheClosesDefenderToBall())
                            {
                                AdvanceToBall(ball);
                            }
                            else
                            {
                                if (!didIReachToGoal)
                                {
                                    var pointNearGoal = GetGoalPosition(true).Value;
                                    pointNearGoal.X += 4f;
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
                            KickToGoal();
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

        private SeenObject FindGoal()
        {
            SeenObject obj;

            while (!m_timeOver)
            {
                // We know where is ball and we can kick it
                // so look for goal
                if (m_side == 'l')
                    obj = m_memory.GetSeenObject("goal r");
                else
                    obj = m_memory.GetSeenObject("goal l");

                if (obj == null)
                {
                    m_robot.Turn(40);
                    m_memory.waitForNewInfo();
                }
                else
                {
                    return obj;
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

            return null;
        }

        private SenseBodyInfo GetBodyInfo()
        {
            m_robot.SenseBody();
            SenseBodyInfo bodyInfo = null;
            while (bodyInfo == null)
            {
                Thread.Sleep(WAIT_FOR_MSG_TIME);
                bodyInfo = m_memory.getBodyInfo();
            }

            return bodyInfo;
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

        private void KickToGoal()
        {
            PointF targetPoint = OpponentGoal;
            if (Utils.GetRandomBoolean())
                targetPoint.Y += 3F;
            else
                targetPoint.Y -= 3F;

            var angleToPoint = CalcAngleToPoint(targetPoint);
            m_robot.Kick(100, angleToPoint);
        }

        private void AdvanceToBall(SeenCoachObject ball)
        {
            float distanceToBall = GetDistanceFrom(ball.Pos.Value);
            float directionToBall = CalcAngleToPoint(ball.Pos.Value);
            bool isDirectionZero = Math.Abs(directionToBall) < 1;

            // If ball is too far then
            // turn to ball or 
            // if we have correct direction then go to ball
            if ((distanceToBall >= 6 && !isDirectionZero) ||
                (distanceToBall < 6 && Math.Abs(directionToBall) > 10))
            {
                m_robot.Turn(directionToBall);
            }
            else
            {
                m_robot.Dash(100);
            }
        }

        public bool AmITheClosesDefenderToBall()
        {
            return FindDefenderClosestToTheBall() == this.m_number;
        }

    }
}
