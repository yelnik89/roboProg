using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace roboProg
{
    class JsonTemplate
    {
        private string[] trafficLightsServer = { "l1", "l2", "l3", "l4" };
        private string[] remotTerminalServer = { "l1", "l2", "l3", "l4" };
        private string[] manipulatorServer = { "s", "n",
            "m1", "m2", "m3", "m4", "m5", "m6",
            "l1", "l2", "l3", "l4", "l5", "l6",
            "t1", "t2", "t3", "t4", "t5", "t6" };
        private string[] politasierServer = { "n", "s",
            "m1", "m2", "m3", "m4",
            "l1", "l2", "l3", "l4",
            "t1", "t2", "t3", "t4" };

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
            if (lintera.Equals("P")) template = this.politasierServer;
            return template;
        }

        public string collectStringData(Dictionary<string, string> thingPropertyInServer, string litera)
        {
            string data = litera;
            if (litera.Equals("P"))
            {
                data += collectPolitasierData(thingPropertyInServer);
            }
            else
            {
                data += collectStringDataAllThings(thingPropertyInServer, litera);
            }
            data += "#";
            data = data.ToLower();
            return data;
        }

        #region politasier
        protected string collectPolitasierData(Dictionary<string, string> thingPropertyInServer)
        {
            string data;
            data = ":" + thingPropertyInServer["X"] + ":" + thingPropertyInServer["Y"] + ":" + thingPropertyInServer["V"] + ":" + thingPropertyInServer["G"];
            return data;
        }
        #endregion

        protected string collectStringDataAllThings(Dictionary<string, string> thingPropertyInServer, string litera)
        {
            string data = "";
            foreach (KeyValuePair<string, string> property in thingPropertyInServer)
            {
                data += ":" + property.Value;
            }
            if (litera.Equals("R")) data += ":0:test";

            return data;
        }
    }
}
