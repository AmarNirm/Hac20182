using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace RoboCup.Infrastracture
{
    public class FlagNameToCoordindates
    {
        private static Coordinates ConvertToGoalPossition(string[] goalStrings)
        {
            Debug.Assert(String.Compare(goalStrings[0], "goal", StringComparison.CurrentCultureIgnoreCase) == 0 );
            if (goalStrings[1] == "l")
            {
                return new Coordinates(Coordinates.LeftLine, 0);
            }
            if (goalStrings[1] == "r")
            {
                return new Coordinates(Coordinates.RightLine, 0);
            }
            throw new NotImplementedException("Cannot handle goal " + goalStrings[1]);
        }

        private static Coordinates ConvertToFlagPossition(string[] flagStrings)
        {
            Debug.Assert(String.Compare(flagStrings[0], "flag", StringComparison.CurrentCultureIgnoreCase) == 0);
            switch (flagStrings[1])
            {
                case ("c"):
                    return getCenterFlagPossition(flagStrings);

                case ("g"):
                    return getGoalFlagPossition(flagStrings);

                case ("p"):
                    return getPeneltyFlagPossition(flagStrings);


                case ("l"):
                    return new Coordinates(Coordinates.LeftEdge, getYPossition(flagStrings));
                case ("r"):
                    return new Coordinates(Coordinates.RightEdge, getYPossition(flagStrings));
                case ("t"):
                    return new Coordinates(getXPossition(flagStrings), Coordinates.TopEdge);
                case ("b"):
                    return new Coordinates(getXPossition(flagStrings), Coordinates.ButtomEdge);
            }

            throw new NotImplementedException("ConvertToFlagPossition " + flagStrings[1]);
        }

        /// <summary>
        /// Returns coordinates for: "flag c t|b| "
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static Coordinates getCenterFlagPossition(string[] flagStrings)
        {
            switch (flagStrings.Length)
            {
                case (2): //flag c
                    return new Coordinates(0, 0);
                case (3): //flag c t|b
                    switch (flagStrings[2])
                    {
                        case ("t"):
                            return new Coordinates(0, Coordinates.TopLine);
                        case ("b"):
                            return new Coordinates(0, Coordinates.ButtomLine);
                    }
                    break;
            }
            throw new NotImplementedException("Cannot handle center flag string: flag c " + flagStrings[2]);
        }


        /// <summary>
        /// Returns coordinates for: "flag g l|r t|b"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static Coordinates getGoalFlagPossition(string[] flagStrings)
        {
            double xPos;
            switch (flagStrings[2])
            {
                case ("l"):
                    xPos = Coordinates.LeftLine;
                    break;
                case ("r"):
                    xPos = Coordinates.RightLine;
                    break;
                default:
                    throw new NotImplementedException("Cannot handle goal flag string: flag g " + flagStrings[2]);
            }
            switch (flagStrings[3])
            {
                case ("t"):
                    return new Coordinates(xPos, Coordinates.TopGoal);
                case ("b"):
                    return new Coordinates(xPos, Coordinates.ButtomGoal);
                default:
                    throw new NotImplementedException("Cannot handle goal flag string: flag g " + flagStrings[2] + " " + flagStrings[3]);
            }
        }

        /// <summary>
        /// Returns coordinates for: "flag p l|r t|c|b"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static Coordinates getPeneltyFlagPossition(string[] flagStrings)
        {
            double xPos;
            
            switch (flagStrings[2])
            {
                case ("l"):
                    xPos = Coordinates.LeftPeneltyArea;
                    break;
                case ("r"):
                    xPos = Coordinates.RightPeneltyArea;
                    break;
                default:
                    throw new NotImplementedException("Cannot handle penelty flag string: flag p " + flagStrings[2]);
            }
            switch (flagStrings[3])
            {
                case ("t"):
                    return new Coordinates(xPos, Coordinates.TopPeneltyArea);
                case ("c"):
                    return new Coordinates(xPos, 0);
                case ("b"):
                    return new Coordinates(xPos, Coordinates.ButtomPeneltyArea);
                default:
                    throw new NotImplementedException("Cannot handle penelty flag string: flag p " + flagStrings[2] + " " + flagStrings[3]);
            }
        }


        /// <summary>
        /// Returns y possition for: "flag l|r 0|t|b 30|20|10"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static double getYPossition(string[] flagStrings)
        {
            switch (flagStrings.Length)
            {
                case (3):
                    switch (flagStrings[2])
                    {
                        case ("t"): return Coordinates.TopLine;
                        case ("0"): return 0;
                        case ("b"): return Coordinates.ButtomLine;
                        default: throw new NotImplementedException("Cannot handle l|r flag string: flag l|r " + flagStrings[2]);
                    }
                case (4):
                    switch (flagStrings[2])
                    {
                        case ("t"): return Coordinates.TopFactor*Int32.Parse(flagStrings[3]);
                        case ("b"): return Coordinates.ButtomFactor*Int32.Parse(flagStrings[3]);
                        default: throw new NotImplementedException("Cannot handle l|r flag string: flag l|r " + flagStrings[2] + flagStrings[3]);
                    }
                default: throw new NotImplementedException("Cannot handle l|r flag string: unexpected length");
            }
        }

        /// <summary>
        /// Returns x possition for: "flag t|b 0|l|r 50|40|30|20|10"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static double getXPossition(string[] flagStrings)
        {
            switch (flagStrings.Length)
            {
                case (3):
                    switch (flagStrings[2])
                    {
                        case ("0"): return 0;
                        default: throw new NotImplementedException("Cannot handle t|b flag string: flag t|b " + flagStrings[2]);
                    }
                case (4):
                    switch (flagStrings[2])
                    {
                        case ("l"): return -1 * Int32.Parse(flagStrings[3]);
                        case ("r"): return Int32.Parse(flagStrings[3]);
                        default: throw new NotImplementedException("Cannot handle t|b flag string: flag t|b " + flagStrings[2] + flagStrings[3]);
                    }
                default: throw new NotImplementedException("Cannot handle l|r flag string: unexpected length");
            }
        }

        /// <summary>
        /// Converts "flag" or "goal" string into Coordinates of X and Y.
        /// The following strings should be supported (With flag prefix or goal with l|r):
        ///  l t; r t; l b; r b; c; c t; c b
        ///  l t 30; l t 20; l t 10; l 0 ; l b 10 ; l b 20; l b 30; 
        ///  r t 30; r t 20; r t 10; r 0 ; r b 10 ; r b 20; r b 30; 
        ///  g l t; g r b; g r t; g r b
        ///  p l t; p l c; p l b
        ///  p r t; p r c; p r b
        ///  t l 50 ... t l 10; t 0; t r 10; ... t r 50
        ///  b l 50 ... b l 10; b 0; b r 10; ... b r 50
        /// </summary>
        /// <param name="flagNameString">flag string with the pattern "flag l t 30"</param>
        /// <returns>Flag Coordinates. In case of error - NotImplementedException will be thrown </returns>
        public static Coordinates Convert(String flagNameString)
        {
            var strings = flagName.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            switch (strings[0].ToLower())
            {
                case "flag":
                    return ConvertToFlagPossition(strings);
                case "goal":
                    return ConvertToGoalPossition(strings);
                default:
                    throw new NotImplementedException("Convert to coordinates only supports 'flag' or 'goal'");
            }
        }
    }
}
