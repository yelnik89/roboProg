using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    class FileReader
    {
        private const string FORMAT = ".txt";

        public string readFile(string name)
        {
            string result = "";
            try
            {
                result = reader(name);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public List<string[]> itemInfo(string name)
        {
            string fileText = "";
            try
            {
                fileText = reader(name);
            }
            catch (Exception)
            {
                throw;
            }
            List<string[]> result = splitString(fileText);
            return result;
        }

        private string reader(string name)
        {
            FileStream file = File.OpenRead(fileName(name));
            byte[] array = new byte[file.Length];
            file.Read(array, 0, array.Length);
            string result = Encoding.Default.GetString(array);

            return result;
        }
        private string fileName(string name)
        {
            return "_" + name + FORMAT;
        }

        private List<string[]> splitString(string text)
        {
            List<string[]> result = new List<string[]>();
            string[] splitStrings = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in splitStrings)
            {
                string[] array = s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                result.Add(array);
            }
            return result;
        }

        public string[] readAuthorizationFile()
        {
            string data;
            string[] result = new string[5];
            try
            {
                data = readFile("authorization");
                string[] splitData = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < 5; i++) result[i] = splitData[i].Trim('\r');
            }
            catch { }
            return result;
        }
    }
}
