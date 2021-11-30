using Newtonsoft.Json;
using System;
using System.Windows;

namespace roboProg
{
    class WorkPrepare
    {
        public WorkPrepare()
        {
            SetRobotSettings();
        }

        private void SetRobotSettings()
        {
            try
            {
                WriteSettingsInStatic(ReadSettings());
            }
            catch(Exception e)
            {
                MessageBox.Show("Не удалось получить настройки" + '\n' + e.Message);
            }
        }

        private string[] ReadSettings()
        {
            FileReader reader = new FileReader();
            string str = reader.readFile("robot_settings");
            return str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
        }

        private void WriteSettingsInStatic(string[] settings)
        {
            int count = settings.Length;
            for (int i = 0; i < count; i++)
            {
                string[] data = settings[i].Split('|');
                SETTINGS.SetRobotSettings(new RobotSettings(data[0], data[1], data[2], data[3].Split(' ')));
            }
        }
    }
}
