using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using RoboCup;
using RoboCup.Entities;
using RoboCup.Infrastructure;
using System.Linq;
using System.Diagnostics;

namespace RoboCup
{

    public class Player : IPlayer
    {
        // Protected members
        protected Robot m_robot;			    // robot which is controled by this brain
        protected Memory m_memory;				// place where all information is stored
        protected PointF m_startPosition;
        volatile protected bool m_timeOver;
        protected Thread m_strategy;
        protected int m_sideFactor
        {
            get
            {
                return m_side == 'r' ? 1 : -1;
            }
        }

        // Public members
        public int m_number;
        public char m_side;
        public String m_playMode;
        public Team m_team;
        public ICoach m_coach;

        public Player(Team team, ICoach coach, bool isGoalie = false)
        {
            m_coach = coach;
            m_memory = new Memory();
            m_team = team;
            m_robot = new Robot(m_memory);
            RandomGenerator = new Random();

            var teamName = isGoalie ? m_team.m_teamName + " (version 6) (goalie)" : m_team.m_teamName;
            m_robot.Init(teamName, out m_side, out m_number, out m_playMode);

            Console.WriteLine("New Player - Team: " + m_team.m_teamName + " Side:" + m_side + " Num:" + m_number);

            m_strategy = new Thread(play);
            m_strategy.Start();
        }

        /// <summary>
        /// Returns absolute target position and angle
        /// </summary>
        /// <returns></returns>
        private Position WhereToGo()
        {
            // get ball position
            var ball = m_coach.GetSeenCoachObject("ball");
            PointF ballPos = new PointF();
            if (ball == null)
            {
            }
            else
            {
                ballPos = ball.Pos.Value;
            }
            // calculate target position
            float alpha = (float)Math.Atan((ballPos.Y - OpponentGoal.Y) / (ballPos.X - OpponentGoal.X));
            var targetX = (float)(ballPos.X - Math.Cos(alpha));
            var targetY = (float)(ballPos.Y + Math.Sin(alpha));
            var targetPos = new PointF(targetX, targetY);

            float alphaDegrees = Utils.RadToDeg(alpha);
            float targetAngle = alphaDegrees;
            if (ball.Pos.Value.X >= 0 && ball.Pos.Value.X < 90 &&
                ball.Pos.Value.Y >= 0 && ball.Pos.Value.Y < 90)
            {

            }
            else if (ball.Pos.Value.X >= 90 && ball.Pos.Value.X < 0 &&
                     ball.Pos.Value.Y >= 0 && ball.Pos.Value.Y < 90)
            {

            }

            return new Position(targetPos, targetAngle);
        }

        public void GoToBall()
        {
            float relAngle;
            while (!Utils.IsSameLocation(GetBall().Pos.Value, GetCurrPlayer().Pos.Value))
            {
                var targetPos = WhereToGo();
                // Turn towards the target position
                relAngle = CalcAngleToPoint(targetPos.Point);
                m_robot.Turn(relAngle);
                Thread.Sleep(100);
                // Run
                RunTowardsPoint(targetPos.Point);
                Thread.Sleep(100);
            }

            // Turn towards the goal
            relAngle = CalcAngleToPoint(OpponentGoal);
            m_robot.Turn(relAngle);
        }

        /// <summary>
        /// Moves to a given position (and angle)
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="targetTowards"></param>
        /// <param name="approximate">If the distance threshold should be bigger</param>
        /// <returns>true if got into the required position (including angle)</returns>
        protected bool MoveToPosition(PointF targetPos, PointF? targetTowards, bool approximate = false,
            bool fast = true)
        {
            if (targetPos != new PointF(0, 0))
            {
                // Check if we need to change the target (in case of a backwards kick)
                var myPos = GetCurrPlayer().Pos.Value;
                var goalPos = GetGoalPosition(false).Value;
                var behind = (m_side == 'l')
                    ? myPos.X > targetPos.X + 0.4 && myPos.X < goalPos.X
                    : myPos.X < targetPos.X - 0.4 && myPos.X > goalPos.X;
                if (behind)
                {
                    //Console.WriteLine("Ball behind: correcting target");
                    if (myPos.Y > targetPos.Y)
                    {
                        targetPos.Y += 1;
                    }
                    else
                    {
                        targetPos.Y -= 1;
                    }
                }
            }

            var distanceThreshold = approximate ? DistanceThreshold + 0.8 : DistanceThreshold;
            float distance = GetDistanceFrom(targetPos);
            Debug.WriteLine($"distance to target = {distance}");
            if (distance > distanceThreshold)
            {
                bool res = TurnTowards(targetPos);
                if (res)
                {
                    if (fast)
                    {
                        m_robot.Dash(100);
                    }
                    else
                    {
                        // Slow down a bit when approaching the target
                        m_robot.Dash(70 * distance);
                    }
                }
            }
            else // Already got to the position, now just need to turn
            {
                if (targetTowards.HasValue)
                {
                    bool res = TurnTowards(targetTowards.Value);
                    if (res)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        protected bool TurnTowards(PointF target)
        {
            float relAngle = CalcAngleToPoint(target);
            float distanceToTarget = GetDistanceFrom(target);

            //if ((distanceToTarget >= 6 && Math.Abs(relAngle) >= AngleThreshold) ||
            //    (distanceToTarget < 6 && Math.Abs(relAngle) > 10))
            if (Math.Abs(relAngle) > AngleThreshold)
            {
                m_robot.Turn(relAngle);
            }
            else
            {
                return true;
            }

            return false;
        }

        public float CalcAngleToPoint(PointF point)
        {
            SeenCoachObject myObj = GetCurrPlayer();
            var myAngle = myObj.BodyAngle.Value;
            var xp = myObj.Pos.Value.X;
            var yp = myObj.Pos.Value.Y;
            float deltaY = point.Y - yp;
            float deltaX = point.X - xp;
            if (deltaX == 0) // Prevent division by 0
            {
                deltaX += 0.001f;
            }

            float alphaPlayerToTargetRad = (float) Math.Atan(deltaY / deltaX);
            float alphaPlayerToTarget = Utils.RadToDeg(alphaPlayerToTargetRad);
            float relAngle = 0;
            if ((deltaY > 0 && deltaX > 0) || (deltaY < 0 && deltaX > 0)) // alpha > 0
            {
                relAngle = -myAngle + alphaPlayerToTarget;
            }
            else // alpha < 0
            {
                relAngle = -myAngle - (180 - alphaPlayerToTarget);
            }

            relAngle = Utils.NormalizeAngle(relAngle);
            // Correct the angle (for the case of deltaX ~ 0)
            var absAngle = Utils.NormalizeAngle(myAngle + relAngle);
            if ((deltaY > 0 && absAngle < 0) || (deltaY < 0 && absAngle > 0))
            {
                relAngle *= -1;
            }

            return relAngle;
        }

        protected void RunTowardsPoint(PointF point)
        {
            var ballDistance = GetDistanceFrom(point);
            m_robot.Dash(10 * ballDistance);
        }

        protected float GetDistanceFrom(PointF point)
        {
            SeenCoachObject myObj = GetCurrPlayer();
            var distance = Utils.Distance(point, myObj.Pos.Value);
            return distance;
        }

        public virtual void play()
        {

        }

        private PointF? m_myGoal;
        public PointF MyGoal
        {
            get
            {
                if (m_myGoal == null)
                {
                    m_myGoal = GetGoalPosition(true);
                }
                if (m_myGoal == null)
                { // The field's width is 105
                    if (m_side == 'r')
                    {
                        return new PointF(52.5f, 0);
                    }
                    else
                    {
                        return new PointF(-52.5f, 0);
                    }
                }
                return m_myGoal.Value;
            }
        }

        private PointF? m_opponentGoal;
        public PointF OpponentGoal
        {
            get
            {
                if (m_opponentGoal == null)
                {
                    m_opponentGoal = GetGoalPosition(false);
                }
                if (m_opponentGoal == null)
                { // The field's width is 105
                    if (m_side == 'r')
                    {
                        return new PointF(-52.5f, 0);
                    }
                    else
                    {
                        return new PointF(52.5f, 0);
                    }
                }
                return m_opponentGoal.Value;
            }
        }

        public Random RandomGenerator { get; set; }

        protected PointF FindAttackerPosition()
        {
            int CloserAttacker = FindAtackerClosestToTheBall();
            var Attacker = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {CloserAttacker}");
            return Attacker.Pos.Value;
        }
        
        public void KickWithProb(PointF target, int power = 100)
        {
            // Kick towards the opponent goal, but pick the exact angle randomly
            var angleToTarget = CalcAngleToPoint(target);
            var rand = RandomGenerator.NextDouble();
            rand -= 0.5;
            rand *= 2; // rand is in [-1,1]
            var kickAngle = angleToTarget + 60 * rand;
            m_robot.Kick(power, kickAngle);
        }

        public void Kick(PointF target, int power = 100)
        {
            TimeSpan timeSinceLastKick = new TimeSpan(DateTime.UtcNow.Ticks) - new TimeSpan(LastKickTime);
            if (timeSinceLastKick.TotalMilliseconds < 500)
            {
                KickWithProb(target);
            }
            else
            {
                var angleToTarget = CalcAngleToPoint(target);
                m_robot.Kick(power, angleToTarget);
            }

            LastKickTime = DateTime.UtcNow.Ticks;
        }

        public bool AmIInMyHalf()
        {
            var myPos = GetCurrPlayer().Pos.Value;
            bool onMySide;
            if (m_side == 'l')
                onMySide = myPos.X <= 5;
            else
                onMySide = myPos.X >= -5;

            return onMySide;
        }

        protected SeenCoachObject GetBall()
        {
            while (!m_timeOver)
            {
                var ball = m_coach.GetSeenCoachObject("ball");
                if (ball == null)
                {
                    Console.WriteLine("ball == null");
                    continue;
                }
                else
                {
                    //Console.WriteLine($"ball: {ball.Pos.Value.X},{ball.Pos.Value.Y}");
                    return ball;
                }
            }
            return null;
        }

        protected SeenCoachObject GetCurrPlayer()
        {
            while (!m_timeOver)
            {
                var currPlayer = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
                if (currPlayer == null)
                {
                    continue;
                }
                else
                {
                    return currPlayer;
                }
            }
            return null;
        }

        protected PointF? GetGoalPosition(bool mine)
        {
            char targetGoal;
            if (mine)
            {
                targetGoal = m_side;
            }
            else
            {
                targetGoal = m_side == 'l' ? 'r' : 'l';
            }
            // get goal position
            var goal = m_coach.GetSeenCoachObject($"goal {targetGoal}");
            PointF goalPos = new PointF();
            if (goal == null)
            {
                return null;
            }
            else
            {
                return goal.Pos.Value;
            }
        }

        public virtual int FindDefenderClosestToTheBall()
        {
            SeenCoachObject ballPosByCoach = null;
            SeenCoachObject Player = null;
            double Distance = -1;
            double CurrentDistance = -1;
            int playerListNum = 0;
            int ClosestPlayerToTheBall = -1;


            var PlayersList = m_coach.GetSeenCoachObjects().Where(kvp => kvp.Value.Name.Contains(m_team.m_teamName)).ToList();

            if (PlayersList.Count < 2)
                return 0;

            Object thisLock = new Object();

            lock (thisLock)
            {

                for (int i = 2; i < 4; i++)
                {
                    playerListNum = i + 1;
                    Player = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {playerListNum}");
                    ballPosByCoach = m_coach.GetSeenCoachObject("ball");


                    Double BallX = ballPosByCoach.Pos.Value.X;
                    Double BallY = ballPosByCoach.Pos.Value.Y;

                                     
                    Double PlayerX = Player.Pos.Value.X;
                    Double PlayerY = Player.Pos.Value.Y;
                    CurrentDistance = Math.Sqrt(Math.Pow(PlayerX - BallX, 2) + Math.Pow(PlayerY - BallY, 2));
                    if (i == 2)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = 3;
                    }
                    else if (CurrentDistance < Distance)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = i+1;
                    }

                }

            }

            return ClosestPlayerToTheBall;
        }

        public virtual int FindAtackerClosestToTheBall()
        {
            SeenCoachObject ballPosByCoach = null;
            SeenCoachObject Player = null;
            double Distance = -1;
            double CurrentDistance = -1;
            int playerListNum = 0;
            int ClosestPlayerToTheBall = -1;


            var PlayersList = m_coach.GetSeenCoachObjects().Where(kvp => kvp.Value.Name.Contains(m_team.m_teamName)).ToList();

            if (PlayersList.Count < 2)
                return 0;

            Object thisLock = new Object();

            lock (thisLock)
            {

                for (int i = 0; i < 2; i++)
                {
                    playerListNum = i + 1;
                    Player = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {playerListNum}");
                    ballPosByCoach = m_coach.GetSeenCoachObject("ball");


                    Double BallX = ballPosByCoach.Pos.Value.X;
                    Double BallY = ballPosByCoach.Pos.Value.Y;


                    Double PlayerX = Player.Pos.Value.X;
                    Double PlayerY = Player.Pos.Value.Y;
                    CurrentDistance = Math.Sqrt(Math.Pow(PlayerX - BallX, 2) + Math.Pow(PlayerY - BallY, 2));
                    if (i == 0)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = 1;
                    }
                    else if (CurrentDistance < Distance)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = i + 1;
                    }

                }

            }

            return ClosestPlayerToTheBall;
        }

        protected float DistanceThreshold = 0.4f;
        protected float AngleThreshold = 3f;
        protected long LastKickTime;
    }
}
