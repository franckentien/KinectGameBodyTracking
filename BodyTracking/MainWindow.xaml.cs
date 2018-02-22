using System;
using System.Collections.Generic;
using System.Globalization;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

namespace BodyTracking
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");
        }

        #endregion

        #region Variable

        //Sensor 
        /// <summary>
        ///     Active Kinect sensor
        /// </summary>
        private static KinectSensor _sensor;

        /// <summary>
        ///     Reader for depth/color/body index frames
        /// </summary>
        private static MultiSourceFrameReader _reader;

        /// <summary>
        ///     List of all body tracked by kinect
        /// </summary>
        private static IList<Body> _bodies;

        //Timer
        /// <summary>
        ///     Timer for Game
        /// </summary>
        private static readonly Timer TimerGame = new Timer();

        //Ressources
        /// <summary>
        ///     A new sound use when user touch point
        /// </summary>
        private static readonly SoundPlayer HitSound = new SoundPlayer(Properties.Resources.hit);

        /// <summary>
        ///     check if game is active
        /// </summary>
        internal static bool GameActive { get; private set; }

        /// <summary>
        ///     Distance necessary between 2 point in meter for say if are close
        /// </summary>
        private const double Distance = 0.1;

        /// <summary>
        ///     Text display in StartButton
        /// </summary>
        private string _startButtonText = "Start Game";

        /// <summary>
        ///     bestScore Players
        /// </summary>
        private static int _bestScore = DeserializeElement();

        /// <summary>
        ///     keep the game active in the game mode 3
        /// </summary>
        private bool _keepActive;

        //gamemode
        /// <summary>
        ///     active game mode
        /// </summary>
        private static byte _gameMode = 1;

        /// <summary>
        ///     number total of game mode created
        /// </summary>
        private const byte NbGameMode = 3;

        /// <summary>
        ///     if the list of point file is readable
        /// </summary>
        private static bool _isReadable;

        /// <summary>
        ///     tab of point for game mode 3
        /// </summary>
        internal static int[] Nextjoint;

        /// <summary>
        ///     ture if active hand in Gamemode 3 is left else false
        /// </summary>
        internal static JointType ActiveHandGm3 = JointType.HandLeft;

        //body
        /// <summary>
        ///     Tab of 6 body will be track by the Sensor
        /// </summary>
        private static readonly TrackBody[] TrackBody = new TrackBody[6];

        /// <summary>
        ///     date when the soft launch for compare date.
        /// </summary>
        private static readonly DateTime DateStart = DateTime.Now;

        //file ressources 
        /// <summary>
        ///     Tab of Stringbuilder for save body information
        /// </summary>
        private readonly StringBuilder[] _csv = new StringBuilder[6];

        /// <summary>
        ///     id of frame for each line of body
        /// </summary>
        private int _frameId;

        /// <summary>
        ///     True if s screen on start game was taken
        /// </summary>
        private static bool _takeScreen;

        /// <summary>
        ///     tab of all line of file loaded with all body information
        /// </summary>
        private string[] _bodyInformation;

        /// <summary>
        ///     tab of all joint of user
        /// </summary>
        private readonly Joint[] _displaybody = new Joint[25];

        #endregion

        #region Event handlers

        /// <summary>
        ///     Load window and active kinect tracker
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();
                _reader =
                    _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth |
                                                       FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
            //best score 
            DeserializeElement();
            BestRecord.Text = "Count: " + _bestScore;
            ChangeModeButtonText.Text = "Game mode: " + _gameMode;
            //game mode 3
            if (_gameMode != 3) return;
            ActiveHandBlock.Visibility = Visibility.Visible;
            Timer.Visibility = Visibility.Hidden;
            ReadFileGm3();
            if (_isReadable) return;
            StartButton.Background = new SolidColorBrush(Colors.Gray);
            DisplayError.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     if windows is closed realser reader and sensor
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            _reader?.Dispose();
            _sensor?.Close();
            SerializeElement();
        }

        #endregion // Event handlers
    }
}