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

        public enum GoalieState
        {
            TurnToGoalLine,
            MoveOnGoalLine,
            TurnToBall
        }

        private bool MovedToGoal = false;
        private bool Init = false;

        public SeenCoachObject MyGoal { get; set; }

        private void TurnToBall()
        {
            var ball = m_memory.GetSeenObject("ball");
            if (ball == null)
            {
                // If you don't know where is ball then find it
                m_robot.Turn(40);
                m_memory.waitForNewInfo();
            }
            else
            {
                // turn to ball
                if (ball.Direction.Value > 0.05)
                {
                    m_robot.Turn(ball.Direction.Value);
                }
                else
                {
                    Init = true;
                }
            }
        }

        private void MoveToGoal()
        {
            var seenGoal = m_memory.GetSeenObject(m_side == 'l' ? "goal l" : "goal r");
            if (seenGoal == null)
            {
                m_robot.Turn(40);
                m_memory.waitForNewInfo();
            }
            else if (seenGoal.Distance.Value > 2)
            {
                // If ball is too far then turn to ball or if we have correct direction then go to ball
                if (seenGoal.Direction.Value != 0)
                    m_robot.Turn(seenGoal.Direction.Value);
                else
                    m_robot.Dash(100);
            }
        }

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
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            CurrentState = GoalieState.TurnToBall;
            
            while (!m_timeOver)
            {
                if (m_playMode == $"free_kick_{m_side}")
                {
                    m_robot.Move(2, 2); // move in the penalty box
                    var me = GetCurrPlayer();
                    float myAngle = me.BodyAngle.Value;
                    float targetAngle = m_side == 'l' ? 0 : 180;
                    float angleDiff = targetAngle - myAngle;
                    if (angleDiff < 0.1)
                    {
                        m_robot.Turn(angleDiff);
                    }
                    else
                    {
                        m_robot.Kick(100, 0); // Kick horizontally towards the opponent goal
                    }
                }
                else
                {
                    if (!Init)
                    {
                        if (!MovedToGoal)
                        {
                            MoveToGoal();
                        }
                        else
                        {
                            TurnToBall();
                        }
                    }
                    else
                    {
                        var ball = m_memory.GetSeenObject("ball");
                        /*switch (CurrentState)
                        {
                            case GoalieState.TurnToBall:
                                if (ball == null)
                                { // Look for the ball
                                    m_robot.Turn(40);
                                    m_memory.waitForNewInfo();
                                }
                                else
                                {
                                    // TODO: if the ball's direction is different
                                    CurrentState = GoalieState.TurnToGoalLine;
                                }
                                break;
                            case GoalieState.TurnToGoalLine:
                                // TODO: turn
                                var me = GetCurrPlayer();
                                float myAngle = me.BodyAngle.Value;
                                float ballLastY = 0;
                                //if (ballLastY)
                                break;
                            case GoalieState.MoveOnGoalLine:
                                break;
                        }
        
                        var myObj = GetCurrPlayer();
                        
                        var goal = GetGoalPosition(false);*/
                        if (ball == null)
                        {
                            CurrentState = GoalieState.TurnToBall;
                        }
                        else if (ball.Distance.Value > 2.0) // If ball is far
                        {
                            /*var me = GetCurrPlayer();
                            float myAngle = me.BodyAngle.Value;
        
                            var myY=me.Pos.Value.Y;
                            var ballY = ball.Pos.Value.Y;
                            // Turn to the ball and move on the goal line
                            m_robot.Turn(ball.Direction.Value);
                            if (myAngle)
                            // Calculate how much to move
        
                            float distanceToMove = 0;
                            m_robot.Dash(10 * distanceToMove);*/
                        }
                        else
                        {
                            // The ball is close -> catch
                            double ballDirection = ball.Direction ?? 0;
                            m_robot.Catch(ballDirection);
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
        }

        public GoalieState CurrentState { get; set; }

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
