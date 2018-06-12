using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using TinMan;

namespace RoboCup
{
    class Program
    {
       static  void Main(string[] args)
        {
            var team1 = new Team(args);
            foreach (var arg in args)
            {
                if (String.Compare(arg, "AddOpponent", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var team2 = new Team(new string[] { "TeamName=DummyOpponent" });
                    break;
                }
            }
            
            Console.ReadKey();
        }

    }
}
