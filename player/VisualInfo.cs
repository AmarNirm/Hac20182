using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using RoboCup.Infrastructure;

namespace RoboCup
{
    public class VisualInfo
    {
        //===========================================================================
        // Public members
        public int m_time;
        public string m_message;
        //public Dictionary<String, SeenObject> m_seenObjects;
        public List<SeenObject> m_seenObjects;

        //===========================================================================
        // Private members
        private char[] m_info;

        //===========================================================================
        // Initialization member functions
        public VisualInfo(String info)
        {
            m_message = info;
            m_seenObjects = new List<SeenObject>();

            info.Trim();
            m_info = info.ToCharArray();
        }

        public void ParseSeeMessage()
        {
            if (!m_message.StartsWith("(see"))
                return;
            
            var objectsMatch = Regex.Matches(m_message, MagicPattern);
            for (int i = 0; i < objectsMatch.Count; i++)
            {
                var obj = objectsMatch[i];
                var innerObjects = obj.Value.Split(')');
                var name = innerObjects[0].Substring(2);
                var parameters = innerObjects[1].Substring(1).Split(' ');
                var floatParams = parameters.Select(strParam => float.Parse(strParam)).ToArray();
                var seenObject = new SeenObject(name, floatParams);
                lock (m_seenObjects)
                {
                    m_seenObjects.Add(seenObject);
                }

            }
        }

        //---------------------------------------------------------------------------
        // This function parses visual information from the server

        public void parse()
        {
            try
            {
                ParseSeeMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private const string MagicPattern = "\\(\\(.*?\\).*?\\)";

    }
}
