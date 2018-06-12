using System;

namespace RoboCup
{
    //***************************************************************************
    //
    //	This interface declares functions which are used to send
    //	command to player
    //
    //***************************************************************************
    public interface ISendCommand
    {
        /// <summary>
        /// Tries to catch the ball in the given direction. This command is permitted only for goalie clients.
        /// The player (goalie) can catch the ball when the ball is in the rectangle with:
        /// width  : goalie_catchable_area_w (1.0),
        /// Length : is goalie_catchable_area_l (2.0),
        /// If the catch is successful, the server goes into free kick mode.
        /// Once it has cought the ball, goalie can move within the penalty box with "move" command.
        /// Allowed once per 5 cycles.
        /// </summary>
        /// <param name="direction">minmoment(-180) ~ maxmoment(180) degrees</param>
        void Catch(double direction);

        /// <summary>
        /// Move the player to the position (X,Y).
        /// This command is available only in the before_kick_off mode, and for the goalie immediately after catching the ball.
        /// 
        /// The origin is the center mark, and the X-axis and Y-axis are toward the opponenet's goal and the right touchline respectively.
        /// Thus, X is usually negative to locate a player in its own side of the field.
        /// </summary>
        /// <param name="x">-52.5 .. 52.5</param>
        /// <param name="y">-34 .. 34</param>
        void Move(double x, double y);

        /// <summary>
        /// Change the direction of the player according to the Moment.
        /// The actual change of direction is reduced when the player is moving quickly.
        /// Allowed only once per cycle
        /// </summary>
        /// <param name="moment">minmoment(-180) ~ maxmoment(180) degrees</param>
        void Turn(double moment);

        /// <summary>
        /// Adds angle to the clients neck angle.
        /// The neck angle is always relative to the direction of the body.
        /// Can be invoked in the same cycle as a turn, dash or kick.
        /// Allowed only once per cycle
        /// </summary>
        /// <param name="angle">minneckmoment(-180) ~ maxneckmoment(180) degrees</param>
        void TurnNeck(double angle);

        /// <summary>
        /// Increases the velocity of the player in the direction it is facing.
        /// If the power is negative, then the player is effectively dashing backwards.
        /// Note: backward dash consumes double stamina.
        /// Allowed only once per cycle
        /// </summary>
        /// <param name="power">minpower(-30) ~ maxpower(100)</param>
        void Dash(double power);

        /// <summary>
        /// Kick the vall with Power and Direction if the ball is near enough (kickable_margin(1.0)+ball_size(0.085)+player_size(0.8)=(~1.88))
        /// This function sends kick command to the server
        /// Allowed only once per cycle
        /// </summary>
        /// <param name="power">minpower(-30) ~ maxpower(100)</param>
        /// <param name="direction">minmoment(-180) ~ maxmoment(180) degrees</param>
        void Kick(double power, double direction);

        /// <summary>
        /// Broadcast message to all players. Message is informed immediatly to clients as sensor information in "hear" format.
        /// Message must be a string with length less than 512 characters, and consists of alphanumeric chars and symbols: +-*/_.()
        /// There is a maximum distance that mmessages ca be heared, see manual for more details.
        /// Allowed *more* than once per cycle
        /// </summary>
        /// <param name="message">a message</param>
        void Say(String message);

        /// <summary>
        /// Change angle of view cone and quality of visual information.
        /// In case of high quality, the server begins to send detailed information about positions of the objects to the client.
        /// In the case of low quality, the server begins to send reduced information about positions (only directions, no distance) of objects to the client.
        ///
        /// The frequency of visual information sent by the server changes according to the angle and the quality:
        /// In the case that angle is normal and quality is high, the information is sent every 150 millisec.
        /// When angle changes to wide, the frequency is halved,
        /// When angle changes to narrow, the frequency is doubled.
        /// For ex. when the angle is narrow and the quality is low, the information is sent every 37.5 millisec.
        /// 
        /// Allowed *more* than once per cycle
        /// </summary>
        /// <param name="width">narrow(=45 degrees) \ normal(=90 degrees)\ wide(=180 degrees)</param>
        /// <param name="quality">high \ low</param>
        void ChangeView(String width, String quality);

        // 
        /**/
        /// <summary>
        /// Retrieves player information ("SenseBodyInfo") from the server.
        /// Use "Memory.getBodyInfo" for result.
        /// Allowed *more* than once per cycle
        /// </summary>
        void SenseBody();

        /// <summary>
        /// This function sends score command to the server, that returns:
        /// (score Time OurScore TheirScore)
        /// Allowed *more* than once per cycle
        /// </summary>
        void Score();
    }
}
