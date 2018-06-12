using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    //***************************************************************************
    //
    //	This class holds visual information about flag
    //
    //***************************************************************************
    public class FlagInfo : ObjectInfo
    {
        //===========================================================================
        // Initialization member functions
        public FlagInfo()
            : base("flag")
        {

        }


        public FlagInfo(char type, char posHoriz, char posVert)
            : base("flag")
        {
            m_flagType = type;
            m_horiz = posHoriz;
            m_vert = posVert;
        }


        //===========================================================================
        // Public members
        char m_flagType;			// 'p'	- penalty area flag
        char m_horiz;			    // l|c|r
        char m_vert;				// t|c|b
    }
}
