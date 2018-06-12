using System.Drawing;

namespace RoboCup.Infrastructure
{
    public class Position
    {
        public PointF Point;
        public float Angle;

        public Position(PointF point, float angle)
        {
            this.Point = point;
            this.Angle = angle;
        }
    }
}
