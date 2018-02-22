using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace BodyTracking
{
    public partial class MainWindow
    {
        #region Game

        /// <summary>
        ///     Display element on windows (_mode, skeleton)
        ///     ask good function for game or counter
        /// </summary>
        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            #region displaymode

            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Camera.Source = frame.ToBitmap();
                }
            }

            #endregion

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame == null) return;
                UpdateDisplayGameTimer(); // update display timer
                RemovePlayer(); //remove inactive player 
                UpdateDisplayPlayer(); //update display player 
                Canvas.Children.Clear(); //reset all display 

                _bodies = new Body[frame.BodyFrameSource.BodyCount];
                frame.GetAndRefreshBodyData(_bodies);

                //display replay 
                if (_bodyInformation != null)
                {
                    LoadBody(_frameId);
                }

                foreach (var body in _bodies)
                {
                    if (body == null) continue;
                    if (!body.IsTracked) continue;
                    //get activeBody id for edit it 
                    var activeBody = GetBodyId(body.TrackingId);

                    //draw skeleton 
                    Canvas.DrawSkeleton(body, _sensor.CoordinateMapper, TrackBody[activeBody], _gameMode);

                    //update all body information 
                    TrackBody[activeBody].EditBody(body, (int) DateTime.UtcNow.Subtract(DateStart).TotalSeconds);

                    if (TrackBody[activeBody].HandLeft.TrackingState == TrackingState.NotTracked &&
                        TrackBody[activeBody].HandRight.TrackingState == TrackingState.NotTracked)
                        continue;
                    if (GameActive)
                    {
                        AddValue(body, activeBody);

                        if (!_takeScreen) TackStartScreen();
                        if (_gameMode == 3)
                        {
                            if (TrackBody[activeBody].Gm3NextPoint == Nextjoint.Length) continue;
                        }
                        if (!IsClose(TrackBody[activeBody])) continue;
                        ChangePoint(TrackBody[activeBody]);
                        DisplayFeedback(TrackBody[activeBody], activeBody);
                    }
                    else
                    {
                        if (CheckIsStay(TrackBody[activeBody])) StartGame();
                    }
                }
                if (GameActive)
                {
                    _frameId++;
                }
            }
        }

        /// <summary>
        ///     Start Game
        ///     Active Timer and change variable
        /// </summary>
        private void StartGame()
        {
            StartButton.Visibility = Visibility.Hidden;
            TextStartButton.Text = _startButtonText;
            if (_gameMode != 3) TimerGame.ActiveTimer(0, 30, 0);
            GameActive = true;
            for (var i = 0; i < TrackBody.Length; i++)
            {
                _csv[i] = new StringBuilder();
                if (TrackBody[i] == null) continue;
                TrackBody[i].Counter = 0;
                var blockCount = (TextBlock) FindName("CountP" + i);
                if (blockCount != null) blockCount.Text = "Count: " + TrackBody[i].Counter;
                TrackBody[i].HandHover = false;
            }
        }

        /// <summary>
        ///     Update BestScore and Reset Values
        /// </summary>
        private void StopGame()
        {
            var bestPlayer = GetBestplayer();
            if (_bestScore < bestPlayer?.Counter)
            {
                _bestScore = bestPlayer.Counter;
                BestRecord.Text = "Count: " + bestPlayer.Counter;
            }
            _startButtonText = "New Game";
            StartButton.Visibility = Visibility.Visible;
            GameActive = false;
            _takeScreen = false;
            _frameId = 0;

            WriteValue();
        }

        /// <summary>
        ///     Change trackBody.active with a new body Joint
        /// </summary>
        /// <param name="trackedBody">trackbody Object </param>
        internal static void ChangePoint(TrackBody trackedBody)
        {
            HashSet<int> exclude;
            IEnumerable<int> range;
            Random rand;
            int index, value1;

            switch (_gameMode)
            {
                case 1:
                    //random with smart point
                    //exclude hand and foot(because foot are to close of ankle
                    exclude = new HashSet<int> {14, 7, 11, 18};
                    range = Enumerable.Range(1, 20).Where(i => !exclude.Contains(i));
                    rand = new Random();
                    index = rand.Next(0, 20 - exclude.Count);
                    value1 = range.ElementAt(index);
                    trackedBody.Active1 = (JointType) value1;
                    break;
                case 2:
                    //Select the first joint in touchable zone 
                    exclude = new HashSet<int> {0, 1, 2, 3, 4, 8, 12, 16, 20};
                    range = Enumerable.Range(1, 20).Where(i => !exclude.Contains(i));
                    rand = new Random();

                    //Select the first point  
                    index = rand.Next(0, 20 - exclude.Count);
                    value1 = range.ElementAt(index);
                    trackedBody.Active1 = (JointType) value1;

                    switch (value1)
                    {
                        case 5: // Left elbow
                            exclude = new HashSet<int> {0, 2, 3, 4, 5, 6, 7, 8, 12, 14, 15, 16, 18, 19, 20};
                            break;
                        case 6: // 	Left wrist
                        case 7: //Left hand
                            exclude = new HashSet<int> {5, 6, 7};
                            break;
                        case 9: // Right elbow
                            exclude = new HashSet<int> {0, 2, 3, 4, 8, 9, 10, 11, 12, 14, 15, 16, 18, 19, 20};
                            break;
                        case 10: //Right wrist
                        case 11: //Right hand
                            exclude = new HashSet<int> {10, 11};
                            break;
                        case 13: //Left knee
                            exclude = new HashSet<int> {1, 12, 13, 14, 15, 16};
                            break;
                        case 14: //Left ankle
                        case 15: //Left foot
                            exclude = new HashSet<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 20};
                                //only 17, 18 ,19
                            break;
                        case 17: // Right knee
                            exclude = new HashSet<int> {1, 12, 16, 17, 18, 19};
                            break;
                        case 18: //Left ankle
                        case 19: //Left foot
                            exclude = new HashSet<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 16, 17, 18, 19, 20};
                                //only 13, 14 ,15
                            break;
                        default:
                            exclude = new HashSet<int>();
                            break;
                    }

                    index = rand.Next(0, 20 - exclude.Count);
                    var value2 = range.ElementAt(index);
                    trackedBody.Active2 = (JointType) value2;
                    break;
                case 3:
                    trackedBody.Active1 = (JointType) Nextjoint[trackedBody.Gm3NextPoint++];
                    break;
            }
            trackedBody.TimerOpacity.StopTimer();
        }

        /// <summary>
        ///     Check if 2 point are near
        /// </summary>
        /// <param name="trackedBody">trackbody Object </param>
        /// <returns>True if are close else False</returns>
        private static bool IsClose(TrackBody trackedBody)
        {
            var pointLeft = trackedBody.HandLeft.Position;
            var pointRight = trackedBody.HandRight.Position;
            var pointGoal1 = trackedBody.GoalJoint1.Position;
            var pointGoal2 = trackedBody.GoalJoint2.Position;

            double distanceX, distanceY, distanceZ;

            switch (_gameMode)
            {
                case 1:
                    distanceX = Math.Abs(pointLeft.X - pointGoal1.X);
                    distanceY = Math.Abs(pointLeft.Y - pointGoal1.Y);
                    distanceZ = Math.Abs(pointLeft.Z - pointGoal1.Z);

                    if (!(distanceX < Distance) || !(distanceY < Distance) || !(distanceZ < Distance*2))
                    {
                        distanceX = Math.Abs(pointRight.X - pointGoal1.X);
                        distanceY = Math.Abs(pointRight.Y - pointGoal1.Y);
                        distanceZ = Math.Abs(pointRight.Z - pointGoal1.Z);
                        return distanceX < Distance && distanceY < Distance && distanceZ < Distance*2;
                    }
                    return true;
                case 2:
                    distanceX = Math.Abs(pointGoal1.X - pointGoal2.X);
                    distanceY = Math.Abs(pointGoal1.Y - pointGoal2.Y);
                    distanceZ = Math.Abs(pointGoal1.Z - pointGoal2.Z);
                    return distanceX < Distance && distanceY < Distance && distanceZ < Distance*2;
                case 3:
                    if (ActiveHandGm3 == JointType.HandLeft)
                    {
                        distanceX = Math.Abs(pointGoal1.X - pointLeft.X);
                        distanceY = Math.Abs(pointGoal1.Y - pointLeft.Y);
                        distanceZ = Math.Abs(pointGoal1.Z - pointLeft.Z);
                    }
                    else
                    {
                        distanceX = Math.Abs(pointGoal1.X - pointRight.X);
                        distanceY = Math.Abs(pointGoal1.Y - pointRight.Y);
                        distanceZ = Math.Abs(pointGoal1.Z - pointRight.Z);
                    }
                    return distanceX < Distance && distanceY < Distance && distanceZ < Distance*2;
                default:
                    return false;
            }
        }

        #endregion
    }
}