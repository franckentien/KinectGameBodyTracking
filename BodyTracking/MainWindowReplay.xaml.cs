using System;
using System.IO;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Win32;

namespace BodyTracking
{
    public partial class MainWindow
    {
        /// <summary>
        ///     open a windows for load file and save it in the tab
        /// </summary>
        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FilterIndex = 1,
                Multiselect = false
            };
            // Set filter options and filter index.

            var userClickedOk = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOk != true) return;
            try
            {
                _bodyInformation = File.ReadAllText(openFileDialog1.FileName).Split('\r');
                if (Checkfile())
                {
                    LoadFile.Visibility = Visibility.Hidden;
                    RemoveFile.Visibility = Visibility.Visible;
                    DisplayError.Visibility = Visibility.Hidden;
                }
                else
                {
                    _bodyInformation = null;
                    DisplayError.Visibility = Visibility.Visible;
                    DisplayError.Text = "this file cannot be use";
                }
            }
            catch
            {
                Console.WriteLine("Can't Read file");
            }
        }

        /// <summary>
        ///     reset body information tab
        /// </summary>
        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            _bodyInformation = null;
            LoadFile.Visibility = Visibility.Visible;
            RemoveFile.Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///     read information of body for drawing it
        /// </summary>
        /// <param name="frameId">number of actual frame</param>
        private void LoadBody(int frameId)
        {
            foreach (var line in _bodyInformation)
            {
                //split the line 
                var bodyFrameInformation = line.Split(',');

                if (bodyFrameInformation.Length == 1) continue;

                if (int.Parse(bodyFrameInformation[0]) != frameId) continue;

                //if the id was biger than the actual frame
                //because this id will be increment i stop the loop 
                if (int.Parse(bodyFrameInformation[0]) > frameId) break;

                //need to read the number of frame in file
                var tempval = 1;

                //until the end of the line 
                while (tempval < bodyFrameInformation.Length)
                {
                    //for all joint 
                    for (var i = 0; i < _displaybody.Length; i++)
                    {
                        //create a new joint with this type 
                        _displaybody[i] = new Joint {JointType = (JointType) i};
                        tempval++;

                        //load is statut tracking 
                        switch (bodyFrameInformation[tempval])
                        {
                            case "Goal":
                                _displaybody[i].TrackingState = TrackingState.Tracked;
                                break;
                            case "Tracked":
                                _displaybody[i].TrackingState = TrackingState.Tracked;
                                break;
                            case "Inferred":
                                _displaybody[i].TrackingState = TrackingState.Inferred;
                                break;
                            default:
                                _displaybody[i].TrackingState = TrackingState.NotTracked;
                                break;
                        }
                        tempval++;

                        //add position to the joint 
                        _displaybody[i].Position.X = float.Parse(bodyFrameInformation[tempval++]);
                        _displaybody[i].Position.Y = float.Parse(bodyFrameInformation[tempval++]);
                        _displaybody[i].Position.Z = float.Parse(bodyFrameInformation[tempval++]);
                    }

                    //draw the skeleton 

                    Canvas.DrawReplay(_displaybody, _sensor.CoordinateMapper);
                }
            }
        }

        /// <summary>
        ///     ///check if load file was ok
        /// </summary>
        /// <returns>true if file was else false </returns>
        private bool Checkfile()
        {
            var idframe = 0;

            foreach (var frameLine in _bodyInformation)
            {
                idframe++;
                var tempCase = 0;
                float testParseFloat;
                int testParseInt;
                var bodyFrameInformation = frameLine.Split(',');

                if (idframe != _bodyInformation.Length)
                    if (bodyFrameInformation.Length < 126)
                    {
                        return false;
                    }

                foreach (var jointValue in bodyFrameInformation)
                {
                    switch (tempCase++)
                    {
                        case 0:

                            if (idframe != _bodyInformation.Length)
                            {
                                if (jointValue == "\n" || jointValue == "\r" || jointValue == "")
                                {
                                    return false;
                                }
                                if (!int.TryParse(jointValue, out testParseInt))
                                {
                                    return false;
                                }
                            }

                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 8:
                        case 9:
                        case 10:
                        case 13:
                        case 14:
                        case 15:
                        case 18:
                        case 19:
                        case 20:
                        case 23:
                        case 24:
                        case 25:
                        case 28:
                        case 29:
                        case 30:
                        case 33:
                        case 34:
                        case 35:
                        case 38:
                        case 39:
                        case 40:
                        case 43:
                        case 44:
                        case 45:
                        case 48:
                        case 49:
                        case 50:
                        case 53:
                        case 54:
                        case 55:
                        case 58:
                        case 59:
                        case 60:
                        case 63:
                        case 64:
                        case 65:
                        case 68:
                        case 69:
                        case 70:
                        case 73:
                        case 74:
                        case 75:
                        case 78:
                        case 79:
                        case 80:
                        case 83:
                        case 84:
                        case 85:
                        case 88:
                        case 89:
                        case 90:
                        case 93:
                        case 94:
                        case 95:
                        case 98:
                        case 99:
                        case 100:
                        case 103:
                        case 104:
                        case 105:
                        case 108:
                        case 109:
                        case 110:
                        case 113:
                        case 114:
                        case 115:
                        case 118:
                        case 119:
                        case 120:
                        case 123:
                        case 124:
                        case 125:
                            if (!float.TryParse(jointValue, out testParseFloat))
                            {
                                return false;
                            }
                            break;
                        case 2:
                        case 7:
                        case 12:
                        case 17:
                        case 22:
                        case 27:
                        case 32:
                        case 37:
                        case 42:
                        case 47:
                        case 52:
                        case 57:
                        case 62:
                        case 67:
                        case 72:
                        case 77:
                        case 82:
                        case 87:
                        case 92:
                        case 97:
                        case 102:
                        case 107:
                        case 112:
                        case 117:
                        case 122:
                            switch (jointValue)
                            {
                                case "Goal":
                                case "Inferred":
                                case "NotTracked":
                                case "Tracked":
                                    break;
                                default:
                                    return false;
                            }
                            break;
                    }
                }
            }
            return true;
        }
    }
}