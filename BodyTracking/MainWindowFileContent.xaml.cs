using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Kinect;

namespace BodyTracking
{
    public partial class MainWindow
    {
        /// <summary>
        ///     tack screen of active windows
        /// </summary>
        private static void TackStartScreen()
        {
            var image = ScreenCapture.CaptureActiveWindow();

            var time = DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);
            var folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Screenshot";
            var file = new DirectoryInfo("Screenshot");
            file.Create();
            var path = Path.Combine(folder, "BodyTracking-" + time + ".png");
            try
            {
                image.Save(path, ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
            _takeScreen = true;
        }

        /// <summary>
        ///     read file for game mode 3 and save value int nextjoin tab
        /// </summary>
        private void ReadFileGm3()
        {
            try
            {
                var line = File.ReadLines("ListJoints.csv").First();
                Nextjoint = line.Split(',').Select(int.Parse).ToArray();
                _isReadable = true;
            }
            catch (FormatException)
            {
                DisplayError.Text = "Error value in 'ListJoints.csv' file";
                _isReadable = false;
            }
            catch (FileNotFoundException)
            {
                DisplayError.Text = "File: 'ListJoints.csv' is not found";
                _isReadable = false;
            }
        }

        #region body information 

        /// <summary>
        ///     add the value to the Stringbuilder
        /// </summary>
        /// <param name="body">all body information</param>
        /// <param name="idBody">id of body in the tabs </param>
        private void AddValue(Body body, int idBody)
        {
            var line = _frameId.ToString();

            foreach (var joint in body.Joints.Values)
            {
                line += "," + joint.JointType;
                //if it's goal 

                if (_gameMode != 2)
                {
                    if (joint.JointType == TrackBody[idBody].Active1)
                    {
                        line += "," + "Goal";
                    }
                    else
                    {
                        line += "," + joint.TrackingState;
                    }
                }
                else
                {
                    if (joint.JointType == TrackBody[idBody].Active1 || joint.JointType == TrackBody[idBody].Active2)
                    {
                        line += "," + "Goal";
                    }
                    else
                    {
                        line += "," + joint.TrackingState;
                    }
                }

                // write value of positions
                line += "," + joint.Position.X;
                line += "," + joint.Position.Y;
                line += "," + joint.Position.Z;
            }

            _csv[idBody].AppendLine(line);
        }

        /// <summary>
        ///     write value in the file
        /// </summary>
        private void WriteValue()
        {
            //create a folder 
            var file = new DirectoryInfo("Body Information");
            file.Create();

            //get actual time for name of the file 
            var time = DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            for (var i = 0; i < _csv.Length; i++)
            {
                if (_csv[i] == null) continue;
                if (_csv[i].ToString() != "")
                {
                    File.WriteAllText("Body Information/" + time + "-" + "Body" + (i + 1) + ".csv", _csv[i].ToString());
                }
            }
        }

        #endregion

        #region Serialization

        /// <summary>
        ///     Save the value in the xml file.
        /// </summary>
        private static void SerializeElement()
        {
            var ser = new XmlSerializer(typeof(XmlElement));
            var myElement = new XmlDocument().CreateElement("bestScore");
            myElement.InnerText = _bestScore.ToString();
            TextWriter writer = new StreamWriter("bestScore.xml");
            ser.Serialize(writer, myElement);
            writer.Close();
        }

        /// <summary>
        ///     take the value in xml file and return it
        /// </summary>
        /// <returns>number of best score or 0</returns>
        private static int DeserializeElement()
        {
            try
            {
                var ser = new XmlSerializer(typeof(XmlElement));
                var reader = new StreamReader("bestScore.xml");
                var p = (XmlElement) ser.Deserialize(reader);
                reader.Close();
                return int.Parse(p.InnerText);
            }
                //if it can read return 0;
            catch
            {
                return 0;
            }
        }

        #endregion
    }
}