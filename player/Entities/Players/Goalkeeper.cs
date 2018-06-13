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
            : base(team, coach, isGoalie:true)
        {
        }

        public enum GoalieState
        {
            TurnToGoalLine,
            MoveOnGoalLine,
            TurnToBall
        }
        
        private bool Init = false;

        private PointF StartingPosition
        {
            get
            {
                if (m_startPosition == null)
                {
                    CalcStartingPosition();
                }
                return m_startPosition;
            }
        }

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

        private PointF CalcStartingPosition()
        {
            float startPosX;
            var goalX = MyGoal.X;
            startPosX = goalX > 0 ? goalX - GoalDistance : goalX + GoalDistance;
            m_startPosition = new PointF(startPosX, 0);
            return m_startPosition;
        }

        public override void play()
        {
            CalcStartingPosition();
            // first move to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            CurrentState = GoalieState.TurnToBall;
            
            while (!m_timeOver)
            {
                if (m_playMode == $"free_kick_{m_side}")
                {
                    m_robot.Move(2, 7); // move in the penalty box
                    // Kick horizontally
                    // TODO: TurnTowards()
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
                        Init = MoveToPosition(StartingPosition, OpponentGoal);
                        if (Init)
                        {
                            Console.WriteLine("Goalkeeper in position!");
                        }
                    }
                    else
                    {
                        //var ball = m_memory.GetSeenObject("ball");
                        var ball = m_coach.GetSeenCoachObject("ball");
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
                        PointF? ballPos = null;
                        if (ball?.Pos != null)
                        {
                            ballPos = ball.Pos.Value;
                        }

                        // TODO: look at ball
                        if (ballPos == null)
                        {
                            CurrentState = GoalieState.TurnToBall;
                        }
                        else if (GetDistanceFrom(ballPos.Value) > 15.0) // If ball is far
                        {
                            TurnTowards(ballPos.Value);
                        }
                        else if (GetDistanceFrom(ballPos.Value) > 1.7) // If ball is close
                        {
                            // TODO: move to the ball! and catch if we're in the penalty box
                        }
                        else // The ball is very close -> catch
                        {
                            double ballDirection = CalcAngleToPoint(ballPos.Value);
                            Console.WriteLine($"Catching in direction {ballDirection}");
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
