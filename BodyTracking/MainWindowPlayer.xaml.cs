using System;
using System.Windows;
using System.Windows.Controls;

namespace BodyTracking
{
    public partial class MainWindow
    {
        #region player

        /// <summary>
        ///     Get the id for trackbody tab of a track body object
        ///     if is unknow create a new object
        /// </summary>
        /// <param name="trackingId">trackingId give by kinect sensor </param>
        /// <returns>index id in the tab of body </returns>
        private static int GetBodyId(ulong trackingId)
        {
            var firstId = 5;
            for (var i = 0; i < TrackBody.Length; i++)
            {
                if (TrackBody[i] == null)
                {
                    // save the lower id null
                    if (firstId > i) firstId = i;
                }
                else
                {
                    if (TrackBody[i].Id == trackingId)
                    {
                        //if the trackid is know return the index id 
                        return i;
                    }
                }
            }
            //if i dont found id i create a new object and return this index in tab
            TrackBody[firstId] = new TrackBody(trackingId, firstId);
            return firstId;
        }

        /// <summary>
        ///     Remove player if is inactive for more 5 seconds
        /// </summary>
        private static void RemovePlayer()
        {
            for (var i = 0; i < TrackBody.Length; i++)
            {
                if (TrackBody[i] == null) continue;
                if ((int) DateTime.UtcNow.Subtract(DateStart).TotalSeconds - TrackBody[i].LastActivity > 5)
                {
                    TrackBody[i] = null;
                }
            }
        }

        /// <summary>
        ///     edit all display information of player in function of gamemode
        /// </summary>
        private void UpdateDisplayPlayer()
        {
            // for all body 
            for (var i = 0; i < TrackBody.Length; i++)
            {
                //reset value and undisplay text
                if (TrackBody[i] == null)
                {
                    var blockPlayer = (TextBlock) FindName("Player" + i);
                    var blockCount = (TextBlock) FindName("CountP" + i);
                    var blockProgbar = (ProgressBar) FindName("ProgBar" + i);
                    if (blockCount != null)
                    {
                        blockCount.Text = "Count: 0";
                        blockCount.Visibility = Visibility.Hidden;
                    }
                    if (blockPlayer != null) blockPlayer.Visibility = Visibility.Hidden;
                    if (blockProgbar != null) blockProgbar.Visibility = Visibility.Hidden;
                }
                //edit text and display text 
                else
                {
                    var blockPlayer = (TextBlock) FindName("Player" + i);
                    var blockCount = (TextBlock) FindName("CountP" + i);

                    if (blockPlayer != null)
                    {
                        blockPlayer.Visibility = Visibility.Visible;
                        blockPlayer.Foreground = TrackBody[i].ColorLine;
                        blockPlayer.Text = TrackBody[i].ColorName;
                    }
                    if (blockCount != null)
                    {
                        blockCount.Visibility = Visibility.Visible;
                        blockCount.Foreground = TrackBody[i].ColorLine;
                    }
                    if (_gameMode != 3)
                    {
                        var blockProgbar = (ProgressBar) FindName("ProgBar" + i);
                        if (blockProgbar != null) blockProgbar.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        var blockProgbar = (ProgressBar) FindName("ProgBar" + i);
                        if (blockProgbar == null || Nextjoint == null) continue;
                        blockProgbar.Visibility = Visibility.Visible;
                        blockProgbar.Value = TrackBody[i].Gm3NextPoint/(double) Nextjoint.Length*100;
                    }
                }
            }
        }

        /// <summary>
        ///     Get the player with the higer score of the game
        /// </summary>
        /// <returns>Best player with TrackBody Object </returns>
        private static TrackBody GetBestplayer()
        {
            var bestScore = 0;
            TrackBody bestPlayer = null;

            foreach (var body in TrackBody)
            {
                if (body == null) continue;
                body.Gm3NextPoint = 0;
                if (body.Counter <= bestScore) continue;
                bestScore = body.Counter;
                bestPlayer = body;
            }
            return bestPlayer;
        }

        /// <summary>
        ///     get player with lower timer
        /// </summary>
        /// <returns>Best player with TrackBody Object</returns>
        private static TrackBody GetLowerTimer()
        {
            var lowerTimer = 4;
            TrackBody lowerPlayer = null;
            foreach (var body in TrackBody)
            {
                if (body == null) continue;
                if (!body.TimerStay.TimerActive) continue;
                if (body.TimerStay.Timers >= lowerTimer) continue;
                lowerTimer = body.Counter;
                lowerPlayer = body;
            }
            return lowerPlayer;
        }

        #endregion
    }
}