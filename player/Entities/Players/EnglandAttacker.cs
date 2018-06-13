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
            Thread.Sleep(10000);
            // first move to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenObject ball;
            SeenObject goal;

            while (!m_timeOver)
            {
                ball = m_memory.GetSeenObject("ball");
                if (ball == null)
                {
                    // If you don't know where is ball then find it
                    m_robot.Turn(-40);
                    m_memory.waitForNewInfo();
                }
                else if (ball.Distance.Value > 1.5)
                {
                    // If ball is too far then
                    // turn to ball or 
                    // if we have correct direction then go to ball
                    if (ball.Direction.Value != 0)
                        m_robot.Turn(ball.Direction.Value);
                    else
                        m_robot.Dash(100);
                }
                else
                {
                    goal = FindGoal();
                    if (goal.Direction.Value < 30)
                    {
                        m_robot.Kick(100, goal.Direction.Value);
                        Console.WriteLine("nirrrrrrrrrrrrrrr");
                    }
                    else
                    {
                        m_robot.Kick(20, goal.Direction.Value);
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
