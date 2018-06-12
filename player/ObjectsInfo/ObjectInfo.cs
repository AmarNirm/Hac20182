using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    //***************************************************************************
    //
    //	This is base class for different classese with visual information
    //	about objects
    //
    //***************************************************************************
    public abstract class ObjectInfo
    {
        //===========================================================================
        // Initialization member functions
        public ObjectInfo(String type)
        {
            m_type = type;
        }


        //===========================================================================
        // Public members
        public String m_type;
        public float m_distance;
        public float m_direction;
        public float m_distChange;
        public float m_dirChange;
    }

}
