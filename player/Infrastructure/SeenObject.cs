using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.Infrastructure
{
    public class SeenObject
    {
        public String Name { get; set; }
        public float? Distance { get; private set; }
        public float? Direction { get; private set; }
        public float? DistChange { get; private set; }
        public float? DirChange { get; private set; }
        public float? BodyFacingDir { get; private set; }
        public float? HeadFacingDir { get; private set; }

        public SeenObject(string name, float[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    Initialize(name, parameters[0]);
                    break;
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

        private void Initialize(string name, float direction)
        {
            Name = name;
            Direction = direction;
        }

        private void Initialize(string name, float distance, float direction)
        {
            Name = name;
            Distance = distance;
            Direction = direction;
        }
        private void Initialize(string name, float distance, float direction, float distChange, float dirChange)
        {
            Name = name;
            Distance = distance;
            Direction = direction;
            DistChange = distChange;
            DirChange = dirChange;
        }
        private void Initialize(string name, float distance, float direction, float distChange,
                                float dirChange, float bodyFacingDir, float headFacingDir)
        {
            Name = name;
            Distance = distance;
            Direction = direction;
            DistChange = distChange;
            DirChange = dirChange;
            BodyFacingDir = bodyFacingDir;
            HeadFacingDir = headFacingDir;
        }
    }
}
