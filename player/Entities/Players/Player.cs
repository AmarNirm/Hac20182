using System;
using System.Drawing;
using System.Threading;
using RoboCup;
using RoboCup.Entities;
using RoboCup.Infrastructure;
using System.Linq;

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

        public Player(Team team, ICoach coach)
        {
            m_coach = coach;
            m_memory = new Memory();
            m_team = team;
            m_robot = new Robot(m_memory);
            m_robot.Init(team.m_teamName, out m_side, out m_number, out m_playMode);

            Console.WriteLine("New Player - Team: " + m_team.m_teamName + " Side:" + m_side +" Num:" + m_number);

            m_strategy = new Thread(play);
            m_strategy.Start();
        }

        public PointF GetGoalPosition(bool mine)
        {
            char targetGoal;
            if (mine)
            {
                targetGoal = m_side;
            }
            else
            {
                targetGoal = m_side=='l'?'r':'l';
            }
            // get goal position
            var goal = m_coach.GetSeenCoachObject($"goal {targetGoal}");
            PointF goalPos = new PointF();
            if (goal == null)
            {
            }
            else
            {
                goalPos = goal.Pos.Value;
            }
            return goalPos;
        }

        /// <summary>
        /// Returns absolute target position and angle
        /// </summary>
        /// <returns></returns>
        private Position WhereToGo()
        {
            // get goal position
            var goalPos = GetGoalPosition(false);
            // get ball position
            var ball = m_coach.GetSeenCoachObject("ball");
            PointF ballPos = new PointF();
            if (ball == null)
            {
            }
            else
            {
                ballPos=ball.Pos.Value;
            }
            // calculate target position
            float alpha = (float) Math.Atan((ballPos.Y - goalPos.Y) / (ballPos.X - goalPos.X));
            var targetX = (float) (ballPos.X - Math.Cos(alpha));
            var targetY = (float) (ballPos.Y + Math.Sin(alpha));
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
            while (!Utils.IsSameLocation(GetBall().Pos.Value, GetCurrPlayer().Pos.Value))
            {
                var targetPos = WhereToGo();
                // Turn towards the target position
                TurnTowardsPoint(targetPos.Point);
                Thread.Sleep(100);
                // Run
                RunTowardsPoint(targetPos.Point);
                Thread.Sleep(100);
            }
            
            // Turn towards the goal
            TurnTowardsPoint(GetGoalPosition(false));
        }

        public void TurnTowardsPoint(PointF point)
        {
            SeenCoachObject myObj = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
            var myAngle = myObj.BodyAngle.Value;
            float alphaPlayerToTarget = (float)Math.Atan((point.Y - myObj.Pos.Value.Y) / (point.X - myObj.Pos.Value.X));
            float alphaPlayerToTargetDegrees = Utils.RadToDeg(alphaPlayerToTarget);
            var relAngle = alphaPlayerToTargetDegrees - myAngle;
            m_robot.Turn(relAngle);
        }

        public void RunTowardsPoint(PointF point)
        {
            SeenCoachObject myObj = m_coach.GetSeenCoachObject($"player {m_team.m_teamName} {m_number}");
            var ballDistance = Utils.Distance(point, myObj.Pos.Value);
            m_robot.Dash(10 * ballDistance);
        }

        public virtual  void play()
        {
 
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
                    Console.WriteLine($"ball: {ball.Pos.Value.X},{ball.Pos.Value.Y}");
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
                    Console.WriteLine("currPlayer == null");
                    continue;
                }
                else
                {
                    Console.WriteLine((string)currPlayer.Name);
                    return currPlayer;
                }
            }
            return null;
        }

               public virtual int FindPlayerClosestToTheBall()
        {
            SeenCoachObject ballPosByCoach = null;
            SeenCoachObject objPlayer = null;
            double Distance = -1;
            double CurrentDistance = -1;
            string player = "player ";
            int playerListNum = 0;
            int ClosestPlayerToTheBall = -1;


            var PlayersList = m_coach.GetSeenCoachObjects().Where(kvp => kvp.Value.Name.Contains(m_team.m_teamName)).ToList();

            if (PlayersList.Count > 2)
                return 0;

            Object thisLock = new Object();

            lock (thisLock)
            {

                for (int i = 0; i < PlayersList.Count; i++)
                {
                    playerListNum = i + 1;
                    ballPosByCoach = m_coach.GetSeenCoachObject("ball");
                    

                    Double BallX = ballPosByCoach.Pos.Value.X;
                    Double BallY = ballPosByCoach.Pos.Value.Y;

                    objPlayer = m_coach.GetSeenCoachObject(player + m_team.m_teamName + " " + playerListNum.ToString());
                    Double PlayerX = objPlayer.Pos.Value.X;
                    Double PlayerY = objPlayer.Pos.Value.Y;
                    CurrentDistance = Math.Sqrt(Math.Pow(PlayerX - BallX, 2) + Math.Pow(PlayerY - BallY, 2));
                    if (i == 1)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = i;
                    }
                    else if (CurrentDistance < Distance)
                    {
                        Distance = CurrentDistance;
                        ClosestPlayerToTheBall = i;
                    }

                }

            }

            return ClosestPlayerToTheBall;
        }
    }
}
