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
    public class EnglandAttackerBack : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;

        public EnglandAttackerBack(Team team, ICoach coach)
            : base(team, coach)
        {
            if (m_side == 'l')
                m_startPosition = new PointF(35, -9);
            else
                m_startPosition = new PointF(-35, -9);
        }

        public override void play()
        {
            SeenCoachObject ball;
            PointF? goal;
            //after gaol, first move to start position
            PointF halfPlace = new PointF(0,0);

            while (!m_timeOver)
            {
                try
                {
                    //go to initial position only after goal or free kick
                    m_robot.Move(m_startPosition.X, m_startPosition.Y);

                    SeenCoachObject currPlayer = GetCurrPlayer();
                    bool onAttackHalf = false;
                    ball = GetBall();

                    if (m_side == 'l')
                        onAttackHalf = currPlayer.Pos.Value.X >= -5; // -5
                    else
                        onAttackHalf = currPlayer.Pos.Value.X <= 5; // 5

                    if (onAttackHalf)
                    {
                        //if (GetDistanceFrom(ball.Pos.Value) <= 9)
                        if (FindAtackerClosestToTheBall() == m_number)
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
                            //AdvanceToStartPoint();
                            GoToOtherWing();
                        }
                    }
                    else
                    {
                        MoveToPosition(halfPlace, null);
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

        public void GoToOtherWing()
        {
            PointF OtherAttacker = FindAttackerPosition();
            var targetTowards = GetBall().Pos.Value;


            if (OtherAttacker.Y >= 5)
                MoveToPosition(new PointF(m_startPosition.X, m_startPosition.Y * -1), targetTowards);
            else
                MoveToPosition(m_startPosition, targetTowards);
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

            if (GetDistanceFrom(targetPoint) < 20)
            {
                Kick(targetPoint);
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
                var range = Math.Abs(OtherAttacker.Pos.Value.X) - Math.Abs(Me.Pos.Value.X);
                if (range > 10 && range <= 25 && Math.Abs(OtherAttacker.Pos.Value.X) > Math.Abs(Me.Pos.Value.X))
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
