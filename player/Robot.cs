using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Threading;
using TinMan;

namespace RoboCup
{
    public class Robot : ISendCommand
    {
        // Private members
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        private Thread m_parser;
        private readonly Memory m_playerMemory;

        public Robot(Memory memory)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(SoccerParams.m_host), SoccerParams.m_port);
            udpClient = new UdpClient();
            m_playerMemory = memory;
        }

        public void Init(string team, out char side, out int num, out String playMode)
        {
            send("(init " + team + ")");
            var result = udpClient.Receive(ref endPoint);

            var message = System.Text.Encoding.Default.GetString(result);
            var splitString = message.Split(' ');

            // We need init token
            if (splitString[0] != "(init")
            {
                throw new Exception(message);
            }

            side = splitString[1].ToCharArray()[0];
            num = int.Parse(splitString[2]);
            playMode = splitString[3].TrimEnd(')', '\n');

            m_parser = new Thread(new ThreadStart(parseSensorInformation));
            m_parser.Start();
        }

        //---------------------------------------------------------------------------
        // This function sends via socket message to the server
        private void send(String message)
        {
            Byte[] sendBytes = new byte[SoccerParams.MSG_SIZE];
            var messageBytes = Encoding.ASCII.GetBytes(message);

            for (int i = 0; i < messageBytes.Length; i++)
            {
                sendBytes[i] = messageBytes[i];
            }
            try
            {
                udpClient.Send(sendBytes, SoccerParams.MSG_SIZE, endPoint);
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
        private void parseSensorInformation()
        {
            String message;
            while (true)
            {
                message = receive();
                //Console.WriteLine($"Received: {message}");
                
                // First check kind of information		
                if (message.StartsWith("(see"))
                {
                    //the robot stores the info in the player's memory
                    m_playerMemory.store(new VisualInfo(message)); 
                }

                if (message.StartsWith("(sense_body"))
                {
                    m_playerMemory.store(new SenseBodyInfo(message));
                }

                if (message.StartsWith("(hear"))
                {
                    parseHear(message);
                }
                    
            }
        }


        //---------------------------------------------------------------------------
        // This function parses hear information
        private void parseHear(String message)
        {
            

            //// get hear information
            //StringTokenizer tokenizer = new StringTokenizer(message, "() ");
            //int time;
            //String sender;

            //// skipp hear token
            //tokenizer.nextToken();
            //time = Integer.parseInt(tokenizer.nextToken());
            //sender = tokenizer.nextToken();
            //if (sender.compareTo("referee") == 0)
            //    m_brain.hear(time, tokenizer.nextToken());
            //else if (sender.compareTo("self") != 0)
            //    m_brain.hear(time, Integer.parseInt(sender), tokenizer.nextToken());
        }

        public void Catch(double direction)
        {
            send("(catch " + direction.ToString() + ")");
        }

        public void Move(double x, double y)
        {
            send("(move " + x.ToString() + " " + y.ToString() + ")");
        }

        public void Turn(double moment)
        {
            send("(turn " + moment.ToString() + ")");
        }

        public void TurnNeck(double angle)
        {
            send("(turn_neck " + angle.ToString() + ")");
        }

        public void Dash(double power)
        {
            send("(dash " + power.ToString() + ")");
        }

        public void Kick(double power, double direction)
        {
            send("(kick " + power.ToString() + " " + direction.ToString() + ")");
        }

        public void Say(string message)
        {
            send("(say " + message + ")");
        }

        public void ChangeView(string width, string quality)
        {
            send("(change_view " + width + " " + quality + ")");
        }


        public void SenseBody()
        {
            m_playerMemory.clearBodyInfo();
            send("(sense_body)");
        }

        public void Score()
        {
            send("(score)");
        }
    }
}
