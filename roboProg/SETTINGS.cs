using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    static class SETTINGS
    {
        private static Dictionary<string, RobotSettings> _robotSettings = new Dictionary<string, RobotSettings>();

        public static Dictionary<string, RobotSettings> RobotSettings
        { get => _robotSettings; }

        public static void SetRobotSettings(RobotSettings robot)
        {
            if (RobotSettings.ContainsKey(robot.Name))
                RobotSettings[robot.Name] = robot;
            else
                RobotSettings.Add(robot.Name, robot);
        }

        public static RobotSettings GetRobotByLitera(string liteta)
        {
            RobotSettings result = null;

            foreach (KeyValuePair<string, RobotSettings> pair in RobotSettings)
            {
                if (!pair.Value.Litera.Equals(liteta))
                    continue;
                result = pair.Value;
            }

            return result;
        }
    }
}
