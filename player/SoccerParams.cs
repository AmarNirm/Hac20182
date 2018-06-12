using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    public static class SoccerParams
    {
       public const int MSG_SIZE = 2048;	        // Size of socket buffer
       public const int MSG_SIZE_COACH = 512;           // Size of socket buffer
       public const int m_port = 6000;          // server port
       public const int m_coachPort = 6001;         // coach port
       public const string m_host = "127.0.0.1";	// Server address
       public const string m_teamName = "England";		// team name
       public const int simulator_step = 100;
    }
}
