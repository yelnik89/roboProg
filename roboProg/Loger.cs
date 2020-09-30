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
        private const string LOGFILE = "log.txt";

        private static Loger loger;

        private Loger()
        { }

        public static Loger getInstance()
        {
            if (loger == null)
                loger = new Loger();
            return loger;
        }


        public void writeLog(string text)
        {
            using (FileStream file = new FileStream(LOGFILE, FileMode.Append))
            {
                byte[] array = prepareText(text);
                file.Write(array, 0, array.Length);
            }
        }

        private byte[] prepareText(string text)
        {
            string s = DateTime.Now.ToString();
            s += ": " + text + '\n';
            byte[] array = Encoding.Default.GetBytes(s);
            return array;
        }
    }
}
