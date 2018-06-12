using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RoboCup.Infrastructure
{
    public class Utils
    {
        public static float Distance(PointF p1, PointF p2)
        {
            return (float) Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static float RadToDeg(float rad)
        {
            return rad * (float) 180.0 / (float) Math.PI;
        }

        public static bool IsSameLocation(PointF value1, PointF value2)
        {
            const float epsilon = (float)0.00001;
            return (value1.X - value2.Y < epsilon) && (value1.Y - value2.Y < epsilon);
        }

        
        /// <summary>
        /// Normalizes the input angle so that it belongs to the interval [-180, 180]
        /// </summary>
        /// <param name="a">the input angle to normalize</param>
        public static float NormalizeAngle(double a)
        {
            if (Math.Abs(a) > 360.0)
            {
                a %= (360.0);
            }
            if (a > 180.0)
            {
                a -= 360.0;
            }
            if (a < -180.0)
            {
                a += 360.0;
            }

            return (float)a;
        }

    }
}
