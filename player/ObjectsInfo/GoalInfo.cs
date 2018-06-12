using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    //***************************************************************************
    //
    //	This class holds visual information about goal
    //
    //***************************************************************************
    public class GoalInfo : ObjectInfo
    {
        //===========================================================================
        // Initialization member functions
        public GoalInfo()
            : base("goal")
        {
        }

        public GoalInfo(char side)
            : base("goal " + side)
        {
        }
    }
}
