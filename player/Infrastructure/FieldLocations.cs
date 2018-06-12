namespace RoboCup.Infrastructure
{
    /// <summary>
    /// Class containing Field Locations for Left (1st to join) team.
    /// Note: for Right(2nd to join) team the Left-Right locations might be inverted.
    /// </summary>
    public static class FieldLocations
    {
        /// X: -52.5 .. 52.5
        /// Y: -34 .. 34
        public const float TopFactor = -1;
        public const float ButtomFactor = 1;

        public const float TopEdge = TopFactor*39;
        public const float ButtomEdge = ButtomFactor*39;
        public const float LeftEdge = (float)-57.5;
        public const float RightEdge = (float)57.5;

        public const float LeftLine = (float)-52.5;
        public const float RightLine = (float)52.5;
        public const float TopLine = (float)(TopFactor *34.0);
        public const float ButtomLine = (float)(ButtomFactor *34.0);

        public const float TopGoal = (float)(TopFactor *7.0);
        public const float ButtomGoal = (float)(ButtomFactor *7.0);

        public const float LeftPeneltyArea = (float)-36.0;
        public const float RightPeneltyArea = (float)36.0;

        public const float TopPeneltyArea = (float)(TopFactor *20.0);
        public const float ButtomPeneltyArea = (float)(ButtomFactor *20.0);
    }
}
