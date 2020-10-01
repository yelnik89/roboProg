using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;

namespace roboProg
{
    class Loger
    {
        private const string LOGFOLDER = "logs";
        private const string OLDLOGSFOLDER = "old_logs";
        private const string FILENAME = "_log";
        private const string FORMAT = ".txt";

        private static Loger loger;

        private Loger()
        {
            prepareLogsFolders();
        }

        public static Loger getInstance()
        {
            if (loger == null)
                loger = new Loger();
            return loger;
        }


        public void writeLog(string text)
        {
            using (FileStream file = new FileStream(LOGFOLDER + "/" + logFile(), FileMode.Append))
            {
                byte[] array = prepareText(text);
                file.Write(array, 0, array.Length);
            }
        }

        private string logFile()
        {
            return DateTime.Today.ToShortDateString() + FILENAME + FORMAT;
        }

        private byte[] prepareText(string text)
        {
            string s = DateTime.Now.ToString();
            s += ": " + text + '\n';
            byte[] array = Encoding.Default.GetBytes(s);
            return array;
        }

        private void prepareLogsFolders()
        {
            checkAndCreateLogsDirectory();
            moveOldLogs();
        }

        private void checkAndCreateLogsDirectory()
        {
            DirectoryInfo logsDirInfo = new DirectoryInfo(LOGFOLDER);
            DirectoryInfo oldLogsDirInfo = new DirectoryInfo(LOGFOLDER + "/" + OLDLOGSFOLDER);
            if (!logsDirInfo.Exists) logsDirInfo.Create();
            if (!oldLogsDirInfo.Exists) oldLogsDirInfo.Create();
        }

        private void moveOldLogs()
        {
            DirectoryInfo logsDirInfo = new DirectoryInfo(LOGFOLDER);
            FileInfo[] logsFiles = logsDirInfo.GetFiles();
            foreach (FileInfo file in logsFiles)
            {
                if (!file.Name.Equals(logFile())) file.MoveTo(LOGFOLDER + "/" + OLDLOGSFOLDER + "/" + file.Name);
            }
        }
    }
}
