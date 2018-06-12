using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Threading;
using TinMan;
using RoboCup.Infrastructure;
using System.Text.RegularExpressions;

namespace RoboCup.Entities
{
    public class Coach : ICoach
    {
        // Private members
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        private Thread m_parser;
        private Dictionary<String, SeenCoachObject> m_seenCoachObjects;



        public Coach()
        {
            m_seenCoachObjects = new Dictionary<String, SeenCoachObject>();
            endPoint = new IPEndPoint(IPAddress.Parse(SoccerParams.m_host), SoccerParams.m_coachPort);
            udpClient = new UdpClient();
            m_parser = new Thread(new ThreadStart(parseCoachInformation));
            m_parser.Start();
        }

        //---------------------------------------------------------------------------
        // This function sends via socket message to the server
        private void send(String message)
        {
            Byte[] sendBytes = new byte[SoccerParams.MSG_SIZE_COACH];
            var messageBytes = Encoding.ASCII.GetBytes(message);

            for (int i = 0; i < messageBytes.Length; i++)
            {
                sendBytes[i] = messageBytes[i];
            }
            try
            {
                udpClient.Send(sendBytes, SoccerParams.MSG_SIZE_COACH, endPoint);
            }
            catch (Exception e)
            {
                Debug.Write("socket sending error " + e.Message);
            }
        }

        //---------------------------------------------------------------------------
        // This function waits for new message from server
        private String receive()
        {
            byte[] buffer = new byte[SoccerParams.MSG_SIZE];
            try
            {
                buffer = udpClient.Receive(ref endPoint);
            }
            catch (Exception e)
            {
                Debug.Write("socket receiving error " + e.Message);
            }
            return (Encoding.ASCII.GetString(buffer));
        }

        //---------------------------------------------------------------------------
        // This function parses sensor information
        private void parseCoachInformation()
        {
            String message;
            while (true)
            {
                Look();
                message = receive();
                //Console.WriteLine($"Received: {message}");
                ParseLookMessage(message);
                Thread.Sleep(SoccerParams.simulator_step);
            }
        }

        public Dictionary<String, SeenCoachObject> GetSeenCoachObjects()
        {
            return m_seenCoachObjects;
        }

        public SeenCoachObject GetSeenCoachObject(string name)
        {
            SeenCoachObject result = null;
            var obj = m_seenCoachObjects.TryGetValue(name, out result);
            return result;
        }

        private void Look()
        {
            send("(look)");
        }

        private void ParseLookMessage(string message)
        {
            if (!message.StartsWith("(ok look"))
                return;

            var objectsMatch = Regex.Matches(message, MagicPattern);
            var tmpDictionary = new Dictionary<String, SeenCoachObject>();
            for (int i = 0; i < objectsMatch.Count; i++)
            {
                var obj = objectsMatch[i];
                var innerObjects = obj.Value.Split(')');
                var name = innerObjects[0].Substring(2);
                var parameters = innerObjects[1].Substring(1).Split(' ');
                var floatParams = parameters.Select(strParam => float.Parse(strParam)).ToArray();
                var seenObject = new SeenCoachObject(name, floatParams);
                tmpDictionary.Add(name, seenObject);
            }
            m_seenCoachObjects = tmpDictionary;
        }

        private const string MagicPattern = "\\(\\(.*?\\).*?\\)";
    }
}
