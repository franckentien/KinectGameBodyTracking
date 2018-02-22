using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Kinect;

namespace BodyTracking
{
    public partial class MainWindow
    {
        #region Menu

        /// <summary>
        ///     Check if hand isHovers StartButton
        /// </summary>
        /// <param name="joint">active hand</param>
        /// <returns>true if the hand is hover else false</returns>
        private static bool IsHover(Joint joint)
        {
            var point = joint.Scale(_sensor.CoordinateMapper);

            return point.X > 650 && point.X < 1270 && point.Y > 450 && point.Y < 630;
        }

        /// <summary>
        ///     Check if hand isHovers StartButton
        /// </summary>
        /// <param name="jointLeft">hand left</param>
        /// <param name="jointRight">hand right</param>
        /// <returns>true if one hand is hover else false</returns>
        private static bool IsHover(Joint jointLeft, Joint jointRight)
        {
            var pointLeft = jointLeft.Scale(_sensor.CoordinateMapper);
            var poinRight = jointRight.Scale(_sensor.CoordinateMapper);

            return (pointLeft.X > 650 && pointLeft.X < 1270 && pointLeft.Y > 450 && pointLeft.Y < 630) ||
                   (poinRight.X > 650 && poinRight.X < 1270 && poinRight.Y > 450 && poinRight.Y < 630);
        }

        /// <summary>
        ///     Check is hand is stay more than 3 seconds hover the StartButton
        /// </summary>
        /// <param name="trackedBody">Trackbody object </param>
        /// <returns>True if is stay more than 3 seconds else False </returns>
        private bool CheckIsStay(TrackBody trackedBody)
        {
            if (GameActive) return false;

            var ishover = false;

            if (_gameMode == 3)
            {
                if (_isReadable)
                    ishover = IsHover(ActiveHandGm3 == JointType.HandLeft ? trackedBody.HandLeft : trackedBody.HandRight);
            }
            else ishover = IsHover(trackedBody.HandLeft, trackedBody.HandRight);

            if (ishover)
            {
                if (trackedBody.HandHover) // hand have been already hover at previous check
                {
                    if (!trackedBody.TimerStay.TimerActive) return true;
                    var lowerPlayer = GetLowerTimer();
                    // update le timer 
                    if (lowerPlayer != null) TextStartButton.Text = lowerPlayer.TimerStay.Timers.ToString();
                    return false;
                }
                trackedBody.HandHover = true;
                trackedBody.TimerStay.ActiveTimer(0, 3, 0);
                //TextStartButton.Text = timers.ToString();
                return false;
            }
            trackedBody.HandHover = false;
            TextStartButton.Text = _startButtonText;
            if (trackedBody.TimerStay.TimerActive) trackedBody.TimerStay.StopTimer();
            return false;
        }

        /// <summary>
        ///     Display some feedback and update text on screen
        /// </summary>
        /// <param name="trackBody">Trackbody object </param>
        /// <param name="activeBody">number of index body for edit the right label </param>
        private void DisplayFeedback(TrackBody trackBody, int activeBody)
        {
            //change counter
            trackBody.Counter++;
            var blockCount = (TextBlock) FindName("CountP" + activeBody);
            if (blockCount != null) blockCount.Text = "Count: " + trackBody.Counter;

            //display audio feedback
            HitSound.Play();

            //display visual feedback
            //move point on screen 
            var point = trackBody.GoalJoint1.Scale(_sensor.CoordinateMapper);
            Hit.Margin = new Thickness(point.X - 50, point.Y - 40, 1920 - point.X - 50, 1080 - point.Y - 40);
            //start animation 
            var sb = FindResource("HitAnimation") as Storyboard;
            if (sb == null) return;
            Storyboard.SetTarget(sb, Hit);
            sb.Begin();
        }

        /// <summary>
        ///     Update time left for game or in Gm3 check if all player have finish.
        /// </summary>
        private void UpdateDisplayGameTimer()
        {
            if (!GameActive) return;
            if (_gameMode == 3)
            {
                _keepActive = false;
                foreach (var t in TrackBody)
                {
                    if (t?.Gm3NextPoint < Nextjoint.Length)
                    {
                        _keepActive = true;
                    }
                }
                if (!_keepActive) StopGame();
            }
            else
            {
                if (TimerGame.TimerActive)
                {
                    Timer.Text = "Timer:" + TimerGame.Timerm.ToString("00") + ":" + TimerGame.Timers.ToString("00");
                }
                else
                {
                    Timer.Text = "Timer:00:00";
                    GameActive = false;
                    StopGame();
                }
            }
        }

        #region gamemode

        /// <summary>
        ///     change Game mode
        /// </summary>
        private void ChangeGameMode(object sender, RoutedEventArgs e)
        {
            //change mode only if game is not active 
            if (!GameActive)
            {
                if (_gameMode >= NbGameMode)
                {
                    _gameMode = 1;
                }
                else
                {
                    _gameMode++;
                }
                ChangeModeButtonText.Text = "Game mode: " + _gameMode;
            }
            //edit display 
            if (_gameMode == 3)
            {
                ActiveHandBlock.Visibility = Visibility.Visible;
                Timer.Visibility = Visibility.Hidden;
                ReadFileGm3();
                if (!_isReadable)
                {
                    StartButton.Background = new SolidColorBrush(Colors.Gray);
                    DisplayError.Visibility = Visibility.Visible;
                }
                else
                {
                    StartButton.Background = new SolidColorBrush(Colors.SkyBlue);
                    DisplayError.Visibility = Visibility.Hidden;
                }
            }
            else //Gm 1, 2
            {
                ActiveHandBlock.Visibility = Visibility.Hidden;
                Timer.Visibility = Visibility.Visible;
                DisplayError.Visibility = Visibility.Hidden;
                StartButton.Background = new SolidColorBrush(Colors.SkyBlue);
            }
        }

        /// <summary>
        ///     click on left hand radio button
        /// </summary>
        private void LeftHand_Checked(object sender, RoutedEventArgs e)
        {
            ActiveHandGm3 = JointType.HandLeft;
        }

        /// <summary>
        ///     click on right hand radio button
        /// </summary>
        private void RightHand_Checked(object sender, RoutedEventArgs e)
        {
            ActiveHandGm3 = JointType.HandRight;
        }

        /// <summary>
        ///     click on change hand button
        ///     active the other hand
        /// </summary>
        private void ChangeActiveHand_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveHandGm3 == JointType.HandLeft)
            {
                ActiveHandGm3 = JointType.HandRight;
                RadioHandRight.IsChecked = true;
            }
            else
            {
                ActiveHandGm3 = JointType.HandLeft;
                RadioHandLeft.IsChecked = true;
            }
        }

        #endregion

        #endregion
    }
}