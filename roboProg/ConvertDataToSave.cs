using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    public class ConvertDataToSave
    {
        private char[] _separators;
        private string _litera;
        private string _whereTo;
        private string[] _template;


        public ConvertDataToSave(string litera, string whereTo)
        {
            _litera = litera;
            _whereTo = whereTo;
            setTemplate();
        }

        private void setTemplate()
        {
            if (_whereTo.Equals("poligon"))
            {
                RobotSettings robotSettings = SETTINGS.GetRobotByLitera(this._litera);
                _template = robotSettings.DataTemplate;
                _separators = new char[] { Convert.ToChar(robotSettings.Separator) };
            }
            else _template = new string[] { };
        }

        public Dictionary<string, string> getDictionary(string data)
        {
            Dictionary<string, string> propertyDictionary = preparationData(data);
            return propertyDictionary;
        }

        private Dictionary<string, string> preparationData(string data)
        {
            char[] separator = { '\n' };
            string[] dataStrings = splitString(data, separator);
            return fullDicrionary(dataStrings);
        }

        private Dictionary<string, string> fullDicrionary(string[] data)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            WriteDataInDictionary(dictionary, data);

            dictionary = writeStatus(dictionary, data[0]);

            return dictionary;
        }

        private void WriteDataInDictionary(Dictionary<string, string> dictionary, string[] data)
        {
            foreach (string dataString in data)
            {
                string str = trimer(dataString);
                string[] propertys = splitString(str, _separators);
                dictionary = writeProperty(propertys, dictionary);
            }
        }

        private Dictionary<string, string> writeProperty(string[] propertys, Dictionary<string, string> dictionary)
        {
            int j = propertys.Length;
            for (int i = 4; i < j; i++)
            {
                string key = propertys[0].ToLower() + (i - 3);
                if (_template.Contains(key))
                    dictionary.Add(key, propertys[i]);
            }
            return dictionary;
        }

        private Dictionary<string, string> writeStatus(Dictionary<string, string> dictionary, string data)
        {
            dictionary.Add("s", data[2].ToString());
            dictionary.Add("n", data[3].ToString());
            return dictionary;
        }

        private string[] splitString(string dataString, char[] separator)
        {
            string[] dataStrings = dataString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return dataStrings;
        }

        private string trimer(string s)
        {
            s = s.Trim(' ');
            s = s.Trim('\n');
            s = s.Trim('#');
            s = s.Trim(' ');
            return s;
        }
    }
}
