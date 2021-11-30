using System;
using System.IO;
using System.Text;

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
            PrepareLogsFolders();
        }

        public static Loger GetInstance()
        {
            if (loger == null)
                loger = new Loger();
            return loger;
        }


        public void WriteLog(string text)
        {
            using (FileStream file = new FileStream(LOGFOLDER + "/" + LogFile(), FileMode.Append))
            {
                byte[] array = prepareText(text);
                file.Write(array, 0, array.Length);
            }
        }

        private string LogFile()
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

        private void PrepareLogsFolders()
        {
            CheckAndCreateLogsDirectory();
            MoveOldLogs();
        }

        private void CheckAndCreateLogsDirectory()
        {
            DirectoryInfo logsDirInfo = new DirectoryInfo(LOGFOLDER);
            DirectoryInfo oldLogsDirInfo = new DirectoryInfo(LOGFOLDER + "/" + OLDLOGSFOLDER);
            if (!logsDirInfo.Exists) logsDirInfo.Create();
            if (!oldLogsDirInfo.Exists) oldLogsDirInfo.Create();
        }

        private void MoveOldLogs()
        {
            DirectoryInfo logsDirInfo = new DirectoryInfo(LOGFOLDER);
            FileInfo[] logsFiles = logsDirInfo.GetFiles();
            foreach (FileInfo file in logsFiles)
            {
                if (!file.Name.Equals(LogFile())) file.MoveTo(LOGFOLDER + "/" + OLDLOGSFOLDER + "/" + file.Name);
            }
        }
    }
}
