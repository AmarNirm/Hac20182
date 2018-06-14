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
    public class EnglandDefenderDown : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;
        private bool Init;
        public EnglandDefenderDown(Team team, ICoach coach)
            : base(team, coach)
        {
            if (m_side == 'l')
            {
                m_startPosition.X = -32;
                m_startPosition.Y = 10;
            }
            else
            {
                m_startPosition.X = 32;
                m_startPosition.Y = 10;
            }
        }

        public override void play()
        {
            // first move to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenCoachObject ball;
            PointF? goal;

            while (!m_timeOver)
            {
                try
                {
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
                            MoveToPosition(ball.Pos.Value, null);
                        }
                        else
                        {
                            goal = GetGoalPosition(false);
                            KickToGoal();
                        }
                    }
                    else
                    {
                        ball = GetBall();
                        var nextPos = new PointF(m_startPosition.X, ball.Pos.Value.Y);
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

        public bool AmITheClosesDefenderToBall()
        {
            return FindDefenderClosestToTheBall() == this.m_number;
        }

    }
}
