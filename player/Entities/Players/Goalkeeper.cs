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
                        // not tested completely
                        /*var ball = m_coach.GetSeenCoachObject("ball");
                        if (ball?.Pos != null)
                        {
                            if (ball.Pos.Value != LastBallPos)
                            {
                                Console.WriteLine("Goalie: ball moved,");
                                m_playMode = "";
                            }
                        }*/

                       // Console.WriteLine("Goalie in free kick mode");
                        float distance = GetDistanceFrom(FreeKickPos);
                        Debug.WriteLine($"distance to FreeKickPos = {distance}");
                        if (distance > DistanceThreshold)
                        {
                           // Console.WriteLine($"Goalie move to {FreeKickPos}");
                            m_robot.Move(FreeKickPos.X, FreeKickPos.Y); // move in the penalty box
                        }
                        else
                        {
                            // Kick horizontally
                           // Console.WriteLine("Goalie in position");
                            var me = GetCurrPlayer();
                            if (TurnTowards(new PointF(OpponentGoal.X, me.Pos.Value.Y)))
                            {
                                //Console.WriteLine("Goalie: kick!");
                                Kick(new PointF(OpponentGoal.X, me.Pos.Value.Y)); // Kick horizontally towards the opponent goal
                                m_playMode = "";
                            }
                        }
                    }
                    else
                    {
                        if (!_init)
                        {
                            _init = MoveToPosition(StartingPosition, OpponentGoal, approximate:true);
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
                            else
                            {
                                // If the ball is about to enter the goal, go to the intersection point and kick!
                                var isBallGoingToGoal = false;
                                PointF predictedBall = new PointF();
                                if (ball.VelX != null && ball.VelY != null)
                                {
                                    var ballVelX = (float) ball.VelX;
                                    var ballVelY = (float) ball.VelY;
                                    float deltaX = (float) (ballVelX / 1.4 * 30);
                                    float deltaY = (float) (ballVelY / 1.4 * 30);
                                    predictedBall = new PointF(ballPos.Value.X + deltaX,
                                        ballPos.Value.Y + deltaY);
                                    //Console.WriteLine($"ballVelX={ballVelX}, ballVelY={ballVelY}");
                                    //Console.WriteLine($"ball in {ballPos}, predicted in {predictedBall}");
                                    isBallGoingToGoal =
                                        Math.Abs(predictedBall.X) >= 52.5 && Math.Abs(predictedBall.Y) <= 7.8;
                                }

                                if (isBallGoingToGoal)
                                {
                                    //Console.WriteLine("Goalie is going to save the day!");
                                    // calc the intersection point
                                    var y = predictedBall.Y;
                                    var target = new PointF(StartingPosition.X, y);
                                    if (MoveToPosition(target, null, approximate: true, fast: true))
                                    {
                                        KickWithProb(OpponentGoal);
                                    }
                                }
                                else if (Utils.Distance(MyGoal, ballPos.Value) > 22.0
                                ) // If the ball is far from the goal, turn to it / return to goal
                                {
                                    var me = GetCurrPlayer();
                                    // If we're far from the starting position (only on X axis), return to it
                                    if (Math.Abs(me.Pos.Value.X - StartingPosition.X) > DistanceThreshold + 0.2)
                                    {
                                        //Console.WriteLine("Goalie: returning to starting position");
                                        MoveToPosition(StartingPosition, ballPos.Value, fast: false);
                                    }
                                    else
                                    {
                                        if (MovingOnGoalLine) // Finish walking
                                        {
                                            if (MoveToPosition(new PointF(StartingPosition.X, TargetY), ballPos.Value,
                                                fast: false))
                                            {
                                                //Console.WriteLine("Goalie: finished moving");
                                                MovingOnGoalLine = false;
                                            }
                                        }
                                        else if (TurnTowards(ballPos.Value))
                                        {
                                            // Check if we should move on the goal line
                                            var myAngleAbs = Math.Abs(me.BodyAngle.Value);
                                            // The absolute angle to the ball can be between 90 and 180 (in case of side=r), so move to [0,90] and normalize:
                                            // [0, 1] when 0=straight, 1=90 degrees (vertical)
                                            var myAngleNorm =
                                                (m_side == 'r') ? (180 - myAngleAbs) / 90 : myAngleAbs / 90;
                                            if (myAngleNorm * ballPos.Value.Y < 0)
                                            {
                                                myAngleNorm *= -1;
                                            }

                                            // decrease the norm, because we prefer to stay close to the center of the goal
                                            myAngleNorm *= 0.8f;
                                            TargetY = HalfGoalLength * myAngleNorm;
                                            if (Math.Abs(me.Pos.Value.Y - TargetY) > 0.7)
                                            {
                                                //Console.WriteLine("Goalie: moving to y=" + TargetY);
                                                MovingOnGoalLine = true;
                                                MoveToPosition(new PointF(StartingPosition.X, TargetY), ballPos.Value,
                                                    fast: false);
                                            }
                                        }
                                    }
                                }
                                else // The ball is close to the goal -> move forward to the ball / catch / kick
                                {
                                    var me = GetCurrPlayer();
                                    double ballDirection = CalcAngleToPoint(ballPos.Value);
                                    //  Catch only if we're in the penalty box and the ball is very close
                                    if (Math.Abs(me.Pos.Value.X - StartingPosition.X) < 12 &&
                                        Math.Abs(me.Pos.Value.Y - StartingPosition.Y) < 12 &&
                                        GetDistanceFrom(ballPos.Value) < 1.7 &&
                                        Math.Abs(ballDirection) < 90)
                                    {
                                        //Console.WriteLine($"Catching in direction {ballDirection}");
                                        var rand = RandomGenerator.NextDouble();
                                        if (rand < 0.50)
                                        {
                                            m_robot.Catch(ballDirection);
                                        }
                                        else
                                        {
                                            // Kick toward the opponent goal, but pick the exact angle randomly
                                            KickWithProb(OpponentGoal);
                                        }
                                    }
                                    else
                                    {
                                        // Move to the ball and kick
                                        if (MoveToPosition(ballPos.Value, null, approximate: true, fast: false))
                                        {
                                            KickWithProb(OpponentGoal);
                                        }
                                    }
                                }
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

        private PointF FreeKickPos { get; set; }
        private string FreeKickModeStr { get; set; }
        public bool MovingOnGoalLine { get; private set; }
        public float TargetY { get; private set; }

        private float HalfGoalLength = 7;
    }
}
