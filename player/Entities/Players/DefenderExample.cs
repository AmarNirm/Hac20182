using RoboCup.Entities;
using RoboCup.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace RoboCup
{
    public class DefenderExample : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;

        public DefenderExample(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 30, 0);
        }

        public override void play()
        {
            // first ,ove to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            while (!m_timeOver)
            {
                SeenObject ball = null;
                SeenObject goal = null;

                //Get current player's info:
                var bodyInfo = GetBodyInfo();
                Console.WriteLine($"Kicks so far : {bodyInfo.Kick}");

                while (ball == null || ball.Distance > 1.5)
                {
                    //Get field information from god (coach).
                    var ballPosByCoach = m_coach.GetSeenCoachObject("ball");
                    if (ballPosByCoach != null && ballPosByCoach.Pos != null)
                    {
                        Console.WriteLine($"Ball Position {ballPosByCoach.Pos.Value.X}, {ballPosByCoach.Pos.Value.Y}");
                    }

                    m_memory.waitForNewInfo();
                    ball = m_memory.GetSeenObject("ball");
                    if (ball == null)
                    {
                        // If you don't know where is ball then find it
                        m_robot.Turn(40);
                        m_memory.waitForNewInfo();
                    }
                    else if (ball.Distance > 1.5)
                    {
                        // If ball is too far then
                        // turn to ball or 
                        // if we have correct direction then go to ball
                        if (Math.Abs((double)ball.Direction) < 0)
                            m_robot.Turn(ball.Direction.Value);
                        else
                            m_robot.Dash(10 * ball.Distance.Value);
                    }
                }

                // We know where is ball and we can kick it
                // so look for goal

                while (goal == null)
                {
                    m_memory.waitForNewInfo();
                    if (m_side == 'l')
                        goal = m_memory.GetSeenObject("goal r");
                    else
                        goal = m_memory.GetSeenObject("goal l");

                    if (goal == null)
                    {
                        m_robot.Turn(40);
                    }
                }

                m_robot.Kick(100, goal.Direction.Value);
            }

            // sleep one step to ensure that we will not send
            // two commands in one cycle.
            try
            {
                Thread.Sleep(SoccerParams.simulator_step);
            }
            catch (Exception e)
            {

            }
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
