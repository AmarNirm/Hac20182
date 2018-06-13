using System;
using System.Diagnostics;
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
            : base(team, coach, isGoalie: true)
        {
            FreeKickModeStr = $"free_kick_{m_side}";
        }

        private bool _init;

        private PointF StartingPosition
        {
            get
            {
                if (m_startPosition.IsEmpty)
                {
                    CalcStartingPosition();
                }

                return m_startPosition;
            }
        }

        private PointF CalcStartingPosition()
        {
            var goalX = MyGoal.X;
            var x = goalX > 0 ? goalX - GoalDistance : goalX + GoalDistance;
            m_startPosition = new PointF(x, 0);
            return m_startPosition;
        }

        private PointF GetFreeKickPosition()
        {
            var x = -50;
            float y = 5;
            if (Utils.GetRandomBoolean())
            {
                y *= -1;
            }
            return new PointF(x, y);
        }

        public override void play()
        {
            CalcStartingPosition();
            // first move to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            FreeKickPos = GetFreeKickPosition();

            while (!m_timeOver)
            {
                try
                {
                    if (m_playMode == FreeKickModeStr)
                    {
                        // TODO: not tested completely
                        /*var ball = m_coach.GetSeenCoachObject("ball");
                        if (ball?.Pos != null)
                        {
                            if (ball.Pos.Value != LastBallPos)
                            {
                                Console.WriteLine("Goalie: ball moved,");
                                m_playMode = "";
                            }
                        }*/

                        Console.WriteLine("Goalie in free kick mode");
                        float distance = GetDistanceFrom(FreeKickPos);
                        Debug.WriteLine($"distance to FreeKickPos = {distance}");
                        if (distance > DistanceThreshold)
                        {
                            Console.WriteLine($"Goalie move to {FreeKickPos}");
                            m_robot.Move(FreeKickPos.X, FreeKickPos.Y); // move in the penalty box
                        }
                        else
                        {
                            // Kick horizontally
                            Console.WriteLine("Goalie in position");
                            var me = GetCurrPlayer();
                            if (TurnTowards(new PointF(OpponentGoal.X, me.Pos.Value.Y)))
                            {
                                Console.WriteLine("Goalie kick!");
                                m_robot.Kick(100, 0); // Kick horizontally towards the opponent goal
                                m_playMode = "";
                            }
                        }
                    }
                    if (m_playMode != FreeKickModeStr) 
                    {
                        if (!_init)
                        {
                            _init = MoveToPosition(StartingPosition, OpponentGoal);
                            if (_init)
                            {
                                Console.WriteLine("Goalkeeper in position!");
                            }
                        }
                        else
                        {
                            var ball = m_coach.GetSeenCoachObject("ball");
                            PointF? ballPos = null;
                            if (ball?.Pos != null)
                            {
                                ballPos = ball.Pos.Value;
                            }

                            if (ballPos == null)
                            {
                            }
                            else if (GetDistanceFrom(ballPos.Value) > 15.0) // If ball is far, turn to it
                            {
                                TurnTowards(ballPos.Value);
                            }
                            else if (GetDistanceFrom(ballPos.Value) > 1.7) // If ball is somewhat close, move forward
                            {
                                // TODO: move to the ball! and catch if we're in the penalty box
                            }
                            else // The ball is very close -> catch
                            {
//                                var memoryBall = m_memory.GetSeenObject("ball");
//                                double ballDirection = memoryBall.Direction.Value;
                                double ballDirection = CalcAngleToPoint(ballPos.Value);
                                Console.WriteLine($"Catching in direction {ballDirection}");
                                m_robot.Catch(ballDirection);
                                // TODO: Determine if we catched the ball - this doesn't work well
//                                if (ballPos.Value == LastBallPos) // then the catch probably succeed
//                                {
//                                    m_playMode = FreeKickModeStr;
//                                }
//                                LastBallPos = ballPos.Value;
                            }
                        }
                    }
                }
                catch
                {
                    // ignored
                }

                // sleep one step to ensure that we will not send
                // two commands in one cycle.
                try
                {
                    Thread.Sleep(2 * SoccerParams.simulator_step);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private PointF LastBallPos { get; set; }
        private PointF FreeKickPos { get; set; }
        private string FreeKickModeStr { get; set; }
    }
}
