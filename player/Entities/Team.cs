using RoboCup.Entities;
using RoboCup.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup
{
    public class Team
    {
        //public members
        public List<Player> m_playerList;
        public  String m_teamName;
        public IFormation m_teamFormation;
        public Coach m_coach;

        public Team(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.ToLower().StartsWith("teamname="))
                {
                    var results = arg.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                    m_teamName = results[1];
                }
            }

            if (m_teamName is null)
            {
                m_teamName = SoccerParams.m_teamName;
            }

            m_coach = new Coach();
            //m_teamFormation = new Formation_4_4_2();
            m_teamFormation = new EnglandFormation();
            m_playerList = m_teamFormation.InitTeam(this, m_coach);
        }

     }
}
