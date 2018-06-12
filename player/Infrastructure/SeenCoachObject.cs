using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace RoboCup.Infrastructure
{
    public class SeenCoachObject
    {
        public String Name { get; set; }
        public PointF? Pos { get; private set; }
        public float? VelX { get; private set; }
        public float? VelY{ get; private set; }
        public float? BodyAngle { get; private set; }
        public float? NeckAngle { get; private set; }

        public SeenCoachObject(string name, float[] parameters)
        {
            switch (parameters.Length)
            {
                case 2:
                    Initialize(name, parameters[0], parameters[1]);
                    break;
                case 4:
                    Initialize(name, parameters[0], parameters[1], parameters[2], parameters[3]);
                    break;
                case 6:
                    Initialize(name, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);
                    break;
            }
        }

        private void Initialize(string name, float posX, float posY)
        {
            Name = name;
            Pos = new PointF(posX, posY);
        }

      
        private void Initialize(string name, float posX, float posY, float velX, float velY)
        {
            Name = name;
            Pos = new PointF(posX, posY);
            VelX = velX;
            VelY = velY;
        }
        private void Initialize(string name, float posX, float posY, float bodyAngle, float neckAngle, float velX, float velY)
        {
            Name = name;
            Pos = new PointF(posX, posY);
            BodyAngle = bodyAngle;
            NeckAngle = neckAngle;
            VelX = velX;
            VelY = velY;
        }
    }
}
