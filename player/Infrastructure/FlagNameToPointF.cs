using System;
using System.Diagnostics;
using System.Drawing;

namespace RoboCup.Infrastructure
{
    public static class FlagNameToPointF
    {
        /// <summary>
        /// Convert flag location to Coordinates.
        /// Returns null in case flag wasn't found
        /// Supported flags:
        ///  l t; r t; l b; r b; c; c t; c b
        ///  l t 30; l t 20; l t 10; l 0 ; l b 10 ; l b 20; l b 30; 
        ///  r t 30; r t 20; r t 10; r 0 ; r b 10 ; r b 20; r b 30; 
        ///  g l t; g r b; g r t; g r b
        ///  p l t; p l c; p l b
        ///  p r t; p r c; p r b
        ///  t l 50 ... t l 10; t 0; t r 10; ... t r 50
        ///  b l 50 ... b l 10; b 0; b r 10; ... b r 50
        /// </summary>
        /// <param name="flagName">Flag name (with the format 'flag l t')</param>
        /// <returns></returns>
        public static PointF? Convert(String flagName)
        {
            try
            {
                var strings = flagName.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                switch (strings[0].ToLower())
                {
                    case "flag":
                        return ConvertToFlagPossition(strings);
                    case "goal":
                        return ConvertToGoalPossition(strings);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error in Converting flag {flagName} to location");
                return null;
            }

        }
        private static PointF? ConvertToGoalPossition(string[] goalStrings)
        {
            Debug.Assert(String.Compare(goalStrings[0], "goal", StringComparison.CurrentCultureIgnoreCase) == 0 );
            if (goalStrings[1] == "l")
            {
                return new PointF(FieldLocations.LeftLine, 0);
            }
            if (goalStrings[1] == "r")
            {
                return new PointF(FieldLocations.RightLine, 0);
            }

            return null;
        }

        private static PointF? ConvertToFlagPossition(string[] flagStrings)
        {
            Debug.Assert(String.Compare(flagStrings[0], "flag", StringComparison.CurrentCultureIgnoreCase) == 0);
            switch (flagStrings[1])
            {
                case ("c"):
                    return GetCenterFlagPossition(flagStrings);

                case ("g"):
                    return GetGoalFlagPossition(flagStrings);

                case ("p"):
                    return GetPeneltyFlagPossition(flagStrings);


                case ("l"):
                    return new PointF(FieldLocations.LeftEdge, GetYPossition(flagStrings));
                case ("r"):
                    return new PointF(FieldLocations.RightEdge, GetYPossition(flagStrings));
                case ("t"):
                    return new PointF(GetXPossition(flagStrings), FieldLocations.TopEdge);
                case ("b"):
                    return new PointF(GetXPossition(flagStrings), FieldLocations.ButtomEdge);
            }

            return null;
        }

        /// <summary>
        /// Returns coordinates for: "flag c t|b| "
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static PointF? GetCenterFlagPossition(string[] flagStrings)
        {
            switch (flagStrings.Length)
            {
                case (2): //flag c
                    return new PointF(0, 0);
                case (3): //flag c t|b
                    switch (flagStrings[2])
                    {
                        case ("t"):
                            return new PointF(0, FieldLocations.TopLine);
                        case ("b"):
                            return new PointF(0, FieldLocations.ButtomLine);
                    }
                    break;
            }

            return null;
        }


        /// <summary>
        /// Returns coordinates for: "flag g l|r t|b"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static PointF? GetGoalFlagPossition(string[] flagStrings)
        {
            float xPos;
            switch (flagStrings[2])
            {
                case ("l"):
                    xPos = FieldLocations.LeftLine;
                    break;
                case ("r"):
                    xPos = FieldLocations.RightLine;
                    break;
                default:
                    return null;
            }
            switch (flagStrings[3])
            {
                case ("t"):
                    return new PointF(xPos, FieldLocations.TopGoal);
                case ("b"):
                    return new PointF(xPos, FieldLocations.ButtomGoal);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns coordinates for: "flag p l|r t|c|b"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static PointF? GetPeneltyFlagPossition(string[] flagStrings)
        {
            float xPos;
            
            switch (flagStrings[2])
            {
                case ("l"):
                    xPos = FieldLocations.LeftPeneltyArea;
                    break;
                case ("r"):
                    xPos = FieldLocations.RightPeneltyArea;
                    break;
                default:
                    return null;
            }
            switch (flagStrings[3])
            {
                case ("t"):
                    return new PointF(xPos, FieldLocations.TopPeneltyArea);
                case ("c"):
                    return new PointF(xPos, 0);
                case ("b"):
                    return new PointF(xPos, FieldLocations.ButtomPeneltyArea);
                default:
                    return null;
            }
        }


        /// <summary>
        /// Returns y possition for: "flag l|r 0|t|b 30|20|10"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static float GetYPossition(string[] flagStrings)
        {
            switch (flagStrings.Length)
            {
                case (3):
                    switch (flagStrings[2])
                    {
                        case ("t"): return FieldLocations.TopLine;
                        case ("0"): return 0;
                        case ("b"): return FieldLocations.ButtomLine;
                        default: throw new Exception("Cannot handle l|r flag string: flag l|r " + flagStrings[2]);
                    }
                case (4):
                    switch (flagStrings[2])
                    {
                        case ("t"): return FieldLocations.TopFactor*Int32.Parse(flagStrings[3]);
                        case ("b"): return FieldLocations.ButtomFactor*Int32.Parse(flagStrings[3]);
                        default: throw new Exception("Cannot handle l|r flag string: flag l|r " + flagStrings[2] + flagStrings[3]);
                    }
                default: throw new Exception("Cannot handle l|r flag string: unexpected length");
            }
        }

        /// <summary>
        /// Returns x possition for: "flag t|b 0|l|r 50|40|30|20|10"
        /// </summary>
        /// <param name="flagStrings"></param>
        /// <returns></returns>
        private static float GetXPossition(string[] flagStrings)
        {
            switch (flagStrings.Length)
            {
                case (3):
                    switch (flagStrings[2])
                    {
                        case ("0"): return 0;
                        default: throw new Exception("Cannot handle t|b flag string: flag t|b " + flagStrings[2]);
                    }
                case (4):
                    switch (flagStrings[2])
                    {
                        case ("l"): return -1 * Int32.Parse(flagStrings[3]);
                        case ("r"): return Int32.Parse(flagStrings[3]);
                        default: throw new Exception("Cannot handle t|b flag string: flag t|b " + flagStrings[2] + flagStrings[3]);
                    }
                default: throw new Exception("Cannot handle l|r flag string: unexpected length");
            }
        }
    }
}
