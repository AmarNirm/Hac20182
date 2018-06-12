using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCup.Infrastracture
{
    public class Coordinates
    {
        /// <param name="x">-52.5 .. 52.5</param>
        /// <param name="y">-34 .. 34</param>
        public const double TopFactor = -1;
        public const double ButtomFactor = 1;

        public const double TopEdge = TopFactor*39;
        public const double ButtomEdge = ButtomFactor*39;
        public const double LeftEdge = -57.5;
        public const double RightEdge = 57.5;

        public const double LeftLine = -52.5;
        public const double RightLine = 52.5;
        public const double TopLine = TopFactor*34.0;
        public const double ButtomLine = ButtomFactor*34.0;

        public const double TopGoal = TopFactor*7.0;
        public const double ButtomGoal = ButtomFactor*7.0;

        public const double LeftPeneltyArea = -36.0;
        public const double RightPeneltyArea = 36.0;

        public const double TopPeneltyArea = TopFactor*20.0;
        public const double ButtomPeneltyArea = ButtomFactor*20.0;


        public double X { get; }

        public double Y { get; }

        public Coordinates() {}

        public Coordinates(double xPos, double yPos)
        {
            X = xPos;
            Y = yPos;
        }
    }
}
