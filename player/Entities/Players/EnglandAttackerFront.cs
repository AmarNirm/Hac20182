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
    public class EnglandAttackerFront : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;

        public EnglandAttackerFront(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 10, 0);
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
                    ball = GetBall();

                    if (GetDistanceFrom(ball.Pos.Value) <= 9)
                    {
                        if (GetDistanceFrom(ball.Pos.Value) <= 1.5)
                        {
                            goal = GetGoalPosition(false);
                            KickToGoal(goal);
                        }
                        MoveToPosition(ball.Pos.Value, null);
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
            PointF upInitialPoint = new PointF(25, 9);
            if (m_side == 'l')
                upInitialPoint.X = 25;
            else
                upInitialPoint.X = -25;

            MoveToPosition(upInitialPoint, null);
        }
        
        private void KickToGoal(PointF? goal)
        {
            PointF targetPoint = OpponentGoal;
            if (Utils.GetRandomBoolean())
                targetPoint.Y += 3F;
            else
                targetPoint.Y -= 3F;

            var angleToPoint = CalcAngleToPoint(targetPoint);

            if (GetDistanceFrom(targetPoint) < 25)
            {
                //Console.WriteLine(angleToPoint);
                m_robot.Kick(100, angleToPoint);
            }
            else
            {
                //check is it better to pass than dribel
                int OtherAttackerNumber;
                if (m_number == 2)
                    OtherAttackerNumber = 3;
                else
                    OtherAttackerNumber = 2;

                var OtherAttacker = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {OtherAttackerNumber}");
                var Me = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
                if (Math.Abs(OtherAttacker.Pos.Value.X) - Math.Abs(Me.Pos.Value.X) > 10)
                    KickTowardsTeamMate(OtherAttacker.Pos.Value);
                else
                    m_robot.Kick(20, CalcAngleToPoint(goal.Value));
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
