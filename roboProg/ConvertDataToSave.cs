using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    public class ConvertDataToSave
    {
        private char[] separators = { ':'};
        private string litera;
        private string whereTo;
        private string[] template;


        public ConvertDataToSave(string litera, string whereTo)
        {
            this.litera = litera;
            this.whereTo = whereTo;
            this.template = getTemplate();
        }
        
        public Dictionary<string, string> getDictionary(string data)
        {
            Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
            propertyDictionary = preparationData(data);
            return propertyDictionary;
        }

        private string[] getTemplate()
        {
            string[] template = { };
            if (this.whereTo.Equals("poligon")) return templateForPoligon();
            return template;
        }
        
        private string[] templateForPoligon()
        {
            RequestJson requestJson = new RequestJson();
            return requestJson.getTemplateFromPoligon(this.litera);
        }

        private Dictionary<string, string> preparationData(string data)
        {
            return politasierFromPoligon(data);
        }

        private Dictionary<string, string> politasierFromPoligon(string data)
        {
            char[] separator = { '\n' };
            string[] dataStrings = splitString(data, separator);
            return fullDicrionary(dataStrings);
        }

        private Dictionary<string, string> fullDicrionary(string[] data)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            char[] separator = { ':' };

            foreach (string dataString in data)
            {
                string str = trimer(dataString);
                string[] propertys = splitString(str, separator);
                dictionary = writeProperty(propertys, dictionary);
            }

            dictionary = writeStatus(dictionary, data[0]);

            return dictionary;
        }

        private Dictionary<string, string> writeProperty(string[] propertys, Dictionary<string, string> dictionary)
        {
            int j = propertys.Length;
            for (int i = 4; i < j; i++)
            {
                string key = propertys[0].ToLower() + (i - 3);
                if (this.template.Contains(key))
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
