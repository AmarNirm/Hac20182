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
    public class EnglandDefenderLeft : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;
        private bool Init;
        public EnglandDefenderLeft(Team team, ICoach coach)
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
            //m_robot.Move(m_startPosition.X, m_startPosition.Y);


            SeenObject ball, goal;

            while (!m_timeOver)
            {
                if (!Init)
                {
                    Init = MoveToPosition(m_startPosition, OpponentGoal);
                    if (Init)
                    {
                        Console.WriteLine("Defender in position!");
                    }
                }

                if(IsBallInMyHalf())
                {
                    ball = m_memory.GetSeenObject("ball");
                    if (ball == null)
                    {
                        // If you don't know where is ball then find it
                        m_robot.Turn(40);
                        m_memory.waitForNewInfo();
                    }
                    else if (ball.Distance.Value > 1.5 && FindPlayerClosestToTheBall() == this.m_number)
                    {
                        // If ball is too far then
                        // turn to ball or 
                        // if we have correct direction then go to ball
                        if (ball.Direction.Value != 0)
                            m_robot.Turn(ball.Direction.Value);
                        else
                            m_robot.Dash(10 * ball.Distance.Value);
                    }
                    else
                    {
                        goal = FindGoal();
                        m_robot.Kick(100, goal.Direction.Value);
                    }
                }
                else
                {
                    var ballInfoFromCoach = m_coach.GetSeenCoachObject("ball");
                    if (ballInfoFromCoach == null)
                    {
                        // If you don't know where is ball then find it
                        //m_robot.Turn(40);
                        //m_memory.waitForNewInfo();
                    }
                    else
                    {
                        //currently not working right due to bug in movetoposition
                        var CurPlayer = this.GetCurrPlayer();
                        var NextPos = new PointF(CurPlayer.Pos.Value.X, ballInfoFromCoach.Pos.Value.Y);
                        //var NextPos = new PointF(CurPlayer.Pos.Value.X, 10);
                        //Console.WriteLine($"NextPos.X={NextPos.X}, NextPos.Y={NextPos.Y}");
                        MoveToPosition(NextPos, OpponentGoal);
                       
                    }

                }
                /*         
                 *         var bodyInfo = GetBodyInfo();


                                */
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
            bool BallInMyHalf = true;
            var ball = m_coach.GetSeenCoachObject("ball");
            if (ball == null)
                //complete 
                ;
            if (m_side == 'l' && ball.Pos.Value.X <= 0 || m_side == 'r' && ball.Pos.Value.X >= 0)
                BallInMyHalf = true;
            else
                BallInMyHalf = false;

            return BallInMyHalf;
        }

     }
}
