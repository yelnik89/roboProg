using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    class RobotSettings
    {
        private string _name;
        private string _litera;
        private string _separator;
        private string[] _dataTemplate;

        public string Name { get => _name; }
        public string Litera { get => _litera; }
        public string Separator { get => _separator; }
        public string[] DataTemplate { get => _dataTemplate; }


        public RobotSettings(string name, string litera, string separator, string[] dataTemplate)
        {
            _name = name;
            _litera = litera;
            _separator = separator;
            _dataTemplate = dataTemplate;
        }

        public string DataTemplateString()
        {
            string result = string.Empty;
            int count = _dataTemplate.Length;
            for (int i = 0; i < count; i++)
                result += _dataTemplate[i] + _separator;
            return result;
        }
    }
}
