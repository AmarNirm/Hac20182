using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    //***************************************************************************
    //
    //	This class holds visual information about player
    //
    //***************************************************************************
    public class PlayerInfo : ObjectInfo
    {
        //===========================================================================
        // Public members
        public String TeamName;
        public int UniformName;

        //===========================================================================
        // Initialization member functions
        public PlayerInfo()
            : base("playe")
        {
        }


        public PlayerInfo(String team, int number)
            : base("player")
        {
            TeamName = team;
            UniformName = number;
        }
    }
}
