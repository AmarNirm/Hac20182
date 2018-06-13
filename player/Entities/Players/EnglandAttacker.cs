using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using RoboCup.Infrastructure;

namespace RoboCup
{
    public class EnglandAttacker : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;

        public EnglandAttacker(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 10, 0);
        }

        public override void play()
        {
            //Thread.Sleep(5000);
            // first move to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenObject ball;
            SeenObject goal;

            while (!m_timeOver)
            {
                try
                {
                    ball = m_memory.GetSeenObject("ball");
                    if (ball == null)
                    {
                        // If you don't know where is ball then find it
                        m_robot.Turn(-40);
                        m_memory.waitForNewInfo();
                    }
                    else
                    {
                        var ballFromCoach = GetBall();
                        bool onAttackerSide = false;
                        if (m_side == 'l')
                            onAttackerSide = ballFromCoach.Pos.Value.X > -5;
                        else
                            onAttackerSide = ballFromCoach.Pos.Value.X < 5;

                        if (onAttackerSide)
                        {
                            if (ball.Distance.Value > 1.5)
                            {
                                AdvanceToBall(ball);
                            }
                            else
                            {
                                goal = FindGoal();
                                KickToGoal(goal);
                            }
                        }
                        else
                        {
                            if (ball.Distance.Value > 1.5)
                            {
                                AdvanceToBall(ball);
                            }
                            else
                            {
                                goal = FindGoal();
                                KickToGoal(goal);
                            }
                        }

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

        private void AdvanceToStartPoint()
        {
            PointF upInitialPoint = new PointF(26, 17);
            //PointF downInitialPoint = new PointF(26, 51);
            MoveToPosition(upInitialPoint, null);
        }

        private void AdvanceToBall(SeenObject ball)
        {
            // If ball is too far then
            // turn to ball or 
            // if we have correct direction then go to ball
            if ((ball.Distance.Value >= 6 && ball.Direction.Value != 0) ||
                (ball.Distance.Value < 6 && ball.Direction.Value > 10))
                m_robot.Turn(ball.Direction.Value);
            else
                m_robot.Dash(100);
        }

        private void KickToGoal(SeenObject goal)
        {
            PointF targetPoint = OpponentGoal;
            if(Utils.GetRandomBoolean())
                targetPoint.Y += 3F;
            else
                targetPoint.Y -= 3F;

            var angleToPoint = CalcAngleToPoint(targetPoint);
            
            if (GetDistanceFrom(targetPoint) < 25)
            {
                Console.WriteLine(angleToPoint);
                m_robot.Kick(100, angleToPoint);
            }
            else
            {
                m_robot.Kick(20, goal.Direction.Value);
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
                    m_robot.Turn(-40);
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
    }
}
