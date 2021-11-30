using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace roboProg
{
    class FileWriter
    {
        private const string FORMAT = ".txt";
        private const string FILENAME = "_robot_settings";


        public bool SaveRobotSettings()
        {
            bool result = true;
            TryWriteStringToFile(GetStringToWrite());
            return result;
        }

        private string GetStringToWrite()
        {
            string str = "";

            foreach (KeyValuePair<string, RobotSettings> robot in SETTINGS.RobotSettings)
            {
                str += robot.Value.Name + "|" + robot.Value.Litera + "|" + robot.Value.Separator + "|" + String.Join(" ", robot.Value.DataTemplate) + '\n';
            }

            return str;
        }

        private bool TryWriteStringToFile(string str)
        {
            bool result = true;
            try
            {
                WriteStringToFile(str);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                result = false;
            }
            return result;
        }

        private void WriteStringToFile(string str)
        {
            using (FileStream file = new FileStream(FileName(), FileMode.Append))
            {
                byte[] array = Encoding.Default.GetBytes(str);
                file.Write(array, 0, array.Length);
            }
        }

        private string FileName()
        {
            return FILENAME + FORMAT;
        }
    }
}
