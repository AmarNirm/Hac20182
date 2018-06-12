using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    public class SenseBodyInfo
    {
        // Public members
        public int Time;
        public String ViewQuality;
        public String ViewWidth;

        public int StaminaValue;
        public int StaminaEffort;

        public int Speed;

        public int Kick;

        public int Dash;

        public int Turn;

        public int Say;

        public SenseBodyInfo(String message)
        {
            ParseMessage(message);
        }

        private void ParseMessage(string message)
        {
            var strings = message.Split(new string[] {") (", "(", ")", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in strings)
            {
                var words = str.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                switch (words[0])
                {
                    case ("sense_body"):
                        Int32.TryParse(words[1], out Time);
                        break;
                    case ("view_mode"):
                        ViewQuality = words[1];
                        ViewWidth = words[2];
                        break;
                    case ("stamina"):
                        Int32.TryParse(words[1], out StaminaValue);
                        Int32.TryParse(words[2], out StaminaEffort);
                        break;
                    case ("speed"):
                        Int32.TryParse(words[1], out Speed);
                        break;
                    case ("kick"):
                        Int32.TryParse(words[1], out Kick);
                        break;
                    case ("dash"):
                        Int32.TryParse(words[1], out Dash);
                        break;
                    case ("turn"):
                        Int32.TryParse(words[1], out Turn);
                        break;
                    case ("say"):
                        Int32.TryParse(words[1], out Say);
                        break;
                }
            }
        }
    }
}