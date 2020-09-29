using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace roboProg
{
    class RequestJson
    {
        private string[] trafficLightsServer = { "l1", "l2", "l3", "l4" };
        private string[] remotTerminalServer = { "l1", "l2", "l3", "l4" };
        private string[] manipulatorServer = { "n", "s", "p",
            "m1", "m2", "m3", "m4", "m5", "m6",
            "l0", "l1", "l2", "l3", "l4", "l5", "l6",
            "t1", "t2", "t3", "t4", "t5", "t6",
            "b1", "b2", "b3", "t", "l" };
        private string[] politasierServer = { "n", "s", "p",
            "m1", "m2", "m3", "m4", "m5",
            "l0", "l1", "l2", "l3", "l4", "l5",
            "t1", "t2", "t3", "t4", "t5",
            "b1", "b2", "b3", "t", "l" };

        private string[] remotTerminalPoligon = { "p", "b1", "b2", "b3", };



        public string getJsonFromServer(string litera)
        {
            string[] template = null;

            if (litera.Equals("L")) template = this.trafficLightsServer;
            if (litera.Equals("R")) template = this.remotTerminalServer;
            if (litera.Equals("G")) template = this.manipulatorServer;
            if (litera.Equals("P")) template = this.politasierServer;

            return JsonConvert.SerializeObject(template);
        }

        public string[] getTemplateFromPoligon(string lintera)
        {
            string[] template = null;
            if (lintera.Equals("R")) template = this.remotTerminalPoligon;
            return template;
        }

        public string collectStringData(Dictionary<string, string> thingPropertyInServer, string litera)
        {
            string data = litera;
            foreach (KeyValuePair<string, string> property in thingPropertyInServer)
            {
                data += ":" + property.Value;
            }
            if (litera.Equals("R")) data += ":0:test";
            data += "#";
            data = data.ToLower();
            return data;
        }
    }
}
