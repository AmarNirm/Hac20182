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
    public class EnglandAttackerDown : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;

        public EnglandAttackerDown(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 10, 0);
        }

        public override void play()
        {
            //Thread.Sleep(5000);
            // first move to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenCoachObject ball;
            PointF? goal;

            while (!m_timeOver)
            {
                try
                {
                    ball = GetBall();
                    bool onAttackerSide = false;
                    if (m_side == 'l')
                        onAttackerSide = ball.Pos.Value.X >= -5; // -5
                    else
                        onAttackerSide = ball.Pos.Value.X <= 5; // 5

                    if (onAttackerSide)
                    {
                        if (GetDistanceFrom(ball.Pos.Value) > 1.5)
                        {
                            MoveToPosition(ball.Pos.Value, null);
                        }
                        else
                        {
                            goal = GetGoalPosition(false);
                            KickToGoal(goal);
                        }
                    }
                    else
                    {
                        AdvanceToStartPoint();
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
            PointF downInitialPoint = new PointF(10, 20);
            if (m_side == 'l')
                downInitialPoint.X = 10;
            else
                downInitialPoint.X *= -1;

            MoveToPosition(downInitialPoint, null);
        }
        
        private void KickToGoal(PointF? goal)
        {
            PointF targetPoint = OpponentGoal;
            if (Utils.GetRandomBoolean())
                targetPoint.Y += 3F;
            else
                targetPoint.Y -= 3F;
            
            if (GetDistanceFrom(targetPoint) < 25)
            {
                //Console.WriteLine(angleToPoint);
                Kick(targetPoint);
            }
            else
            {
                //check is it better to pass than dribel
                int OtherAttackerNumber;
                if (m_number == 1)
                    OtherAttackerNumber = 2;
                else
                    OtherAttackerNumber = 1;

                var OtherAttacker = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {OtherAttackerNumber}");
                var Me = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
                if (Math.Abs(OtherAttacker.Pos.Value.X) - Math.Abs(Me.Pos.Value.X) > 10)
                    Kick(OtherAttacker.Pos.Value);
                else
                    Kick(goal.Value, 20);
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

