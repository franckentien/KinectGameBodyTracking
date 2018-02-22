using System.Windows.Media;
using Microsoft.Kinect;

namespace BodyTracking
{
    public class TrackBody
    {
        #region constructor 

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id">id body kineck</param>
        /// <param name="player">id player in tab</param>
        public TrackBody(ulong id, int player)
        {
            Id = id;
            switch (player)
            {
                case 0:
                    ColorName = "Green";
                    ColorLine = new SolidColorBrush(Colors.Lime);
                    break;
                case 1:
                    ColorName = "Pink";
                    ColorLine = new SolidColorBrush(Colors.DeepPink);
                    break;
                case 2:
                    ColorName = "Bleu";
                    ColorLine = new SolidColorBrush(Colors.Blue);
                    break;
                case 3:
                    ColorName = "Red";
                    ColorLine = new SolidColorBrush(Colors.Red);
                    break;
                case 4:
                    ColorName = "Yellow";
                    ColorLine = new SolidColorBrush(Colors.Yellow);
                    break;
                case 5:
                    ColorName = "White";
                    ColorLine = new SolidColorBrush(Colors.White);
                    break;
            }
        }

        #endregion

        #region function 

        /// <summary>
        ///     Update all data information for body
        /// </summary>
        /// <param name="body">all inforn\mation of body is tracked</param>
        /// <param name="lastActivity">int for the actual timestamp</param>
        public void EditBody(Body body, int lastActivity)
        {
            HandLeft = body.Joints[JointType.HandLeft];
            HandRight = body.Joints[JointType.HandRight];
            GoalJoint1 = body.Joints[Active1];
            GoalJoint2 = body.Joints[Active2];
            LastActivity = lastActivity;
        }

        #endregion

        #region variable

        //sensor variable 
        /// <summary>
        ///     TrackingId of sensor
        /// </summary>
        internal ulong Id { get; private set; }

        //personal variable 
        /// <summary>
        ///     personal color line for drawing
        /// </summary>
        internal SolidColorBrush ColorLine { get; private set; }

        /// <summary>
        ///     displaying a user name
        /// </summary>
        internal string ColorName { get; private set; }

        //body information 
        /// <summary>
        ///     Hand Right Joint
        /// </summary>
        internal Joint HandRight { get; private set; }

        /// <summary>
        ///     Hand Left Joint
        /// </summary>
        internal Joint HandLeft { get; private set; }

        /// <summary>
        ///     Goal Joint
        /// </summary>
        internal Joint GoalJoint1 { get; private set; }

        /// <summary>
        ///     Goal Joint
        /// </summary>
        internal Joint GoalJoint2 { get; private set; }

        /// <summary>
        ///     true if hand is hover start button
        /// </summary>
        internal bool HandHover = false;

        /// <summary>
        ///     last activity track of user
        /// </summary>
        internal int LastActivity { get; private set; }

        //display variable 
        /// <summary>
        ///     Get active Joint for goal Joint
        /// </summary>
        internal JointType Active1 = 0;

        /// <summary>
        ///     Get active Joint for goal Joint
        /// </summary>
        internal JointType Active2;

        /// <summary>
        ///     get number of touch in a game
        /// </summary>
        internal int Counter;

        /// <summary>
        ///     value of point need to be display in tab for gamemode3
        /// </summary>
        internal int Gm3NextPoint = 0;

        //timer 
        /// <summary>
        ///     timer for edit opacity and change point if is not touch
        /// </summary>
        internal readonly Timer TimerOpacity = new Timer("Opacity");

        /// <summary>
        ///     timer for handhover
        /// </summary>
        internal readonly Timer TimerStay = new Timer();

        #endregion
    }
}