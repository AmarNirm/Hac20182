using System;

namespace RoboCup
{
    //***************************************************************************
    //
    //	This interface declares functions which are used to get information
    //	from the enviroment
    //
    //***************************************************************************
    public interface ISensorInput
    {
        //---------------------------------------------------------------------------
        // This function sends see information
        void see(VisualInfo info);

        //---------------------------------------------------------------------------
        // This function receives hear information from player
        void hear(int time, int direction, String message);

        //---------------------------------------------------------------------------
        // This function receives hear information from referee
        void hear(int time, String message);
    }
}
