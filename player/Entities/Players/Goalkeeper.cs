using System;
using System.Drawing;
using System.Threading;
using RoboCup;
using RoboCup.Entities;
using RoboCup.Infrastructure;

namespace player.Entities.Players
{
    /// <summary>
    /// Also called goalie, or keeper
    /// The goalkeeper is simply known as the guy with gloves who keeps the opponents from scoring. He has a special position because only him can play the ball with his hands (provided that he is inside his own penalty area and the ball was not deliberately passed to him by a team mate).
    /// Aside from being the last line of defense, the goalkeeper is the first person in attack. That is why keepers who can make good goal kicks and strategic ball throws to team mates are valuable.
    /// The goalie has four main roles: saving, clearing, directing the defense, and distributing the ball. Saving is the act of preventing the ball from entering the net while clearing means keeping the ball far from the goal area.
    /// The goalkeeper has the role of directing the defense since he is the farthest player at the back and he can see where the defenders should position themselves.
    /// Distributing the ball happens when a goalkeeper decides whether to kick the ball or throw it after making a save. Where the keeper throws or kicks the ball is the first instance of attack
    /// </summary>
    public class Goalkeeper : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;
        private const float GoalDistance = (float) 1.5;

        public Goalkeeper(Team team, ICoach coach)
            : base(team, coach)
        {
        }

        public SeenCoachObject MyGoal { get; set; }

        public override void play()
        {
            MyGoal = m_coach.GetSeenCoachObject(m_side == 'l' ? "goal l" : "goal r");
            float start_pos_x;
            if (MyGoal != null)
            {
                var goal_x = MyGoal.Pos.Value.X;
                start_pos_x = goal_x > 0 ? goal_x - GoalDistance : goal_x + GoalDistance;
                Console.WriteLine("Golie going to " + start_pos_x);
            }
            else
            {
                start_pos_x = m_side == 'l' ? -51:51;
                Console.WriteLine("Golie going to " + start_pos_x);
            }

            m_startPosition = new PointF(start_pos_x, 0);
            // first move to start position
            // m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenObject obj;

            while (!m_timeOver)
            {
                //m_robot.Move(m_startPosition.X, m_startPosition.Y);
                var myObj = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
                if (myObj != null)
                {
                    Console.WriteLine(myObj.BodyAngle);
                    m_robot.Turn(45);
                    Console.WriteLine(myObj.BodyAngle);
                }
                
                //var bodyInfo = GetBodyInfo();

                //obj = m_memory.GetSeenObject("ball");
                /*if (obj == null)
                {
                    // If you don't know where is ball then find it
                    m_robot.Turn(40);
                    m_memory.waitForNewInfo();
                }
                else if (obj.Distance.Value > 1.5)
                {
                    // If ball is too far then
                    // turn to ball or 
                    // if we have correct direction then go to ball
                    if (obj.Direction.Value != 0)
                        m_robot.Turn(obj.Direction.Value);
                    else
                        m_robot.Dash(10 * obj.Distance.Value);
                }
                else
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
                        m_robot.Kick(100, obj.Direction.Value);
                }*/

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
