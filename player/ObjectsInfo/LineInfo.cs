using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    //***************************************************************************
    //
    //	This class holds visual information about line
    //
    //***************************************************************************
    public class LineInfo : ObjectInfo
    {
        //===========================================================================
        // Initialization member functions
        public LineInfo()
            : base("line")
        {
        }


        public LineInfo(char kind)
            : base("line")
        {
            m_kind = kind;
        }


        //===========================================================================
        // Public members
        char m_kind;				// l|r|t|b
    }
}
