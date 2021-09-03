using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace roboProg
{
    class WorkCycle
    {
        private bool cyclicalRun = false;
        private Dictionary<string, string>[] thingsPropertyInServer;
        private Dictionary<string, string>[] thingsPropertyInPolygon;
        private List<string[]> teamSettings;
        private Messenger messenger;
        private RequestJson json;
        private Loger loger;
        

        public void Main()
        {
            this.loger = Loger.getInstance();
            this.json = new RequestJson();

        }
        public Dispatcher getDispatcher()
        {
            return Dispatcher.CurrentDispatcher;
        }

        private void CycleMethod(string address, string authInfo, string authorizationType)
        {
            this.messenger = new Messenger(authInfo, address, authorizationType);
            int teamListLenght = this.teamSettings.Count();

            cyclicalRequest(messenger, teamListLenght);
        }

        private void cyclicalRequest(Messenger messenger, int teamListLenght)
        {
            for (int i = 0; i < teamListLenght; i++)
            {
                dataTransfer(messenger, this.teamSettings[i], i);
                if (!this.cyclicalRun) break;
            }
        }

        private void dataTransfer(Messenger messenger, string[] thing, int indexOfThing)
        {
            if (cachProperty(messenger, thing, indexOfThing))
            {
                sendUDP(thing, indexOfThing);
                trySendPropertyToServer(messenger, indexOfThing);
            }
        }

        private bool cachProperty(Messenger messenger, string[] thing, int indexOfThing)
        {
            bool result = true;
            try
            {
                property(messenger, thing, indexOfThing);
            }
            catch (Exception e)
            {
                this.loger.writeLog(e.Message);
                result = false;
            }

            return result;
        }

        private async Task property(Messenger messenger, string[] thing, int indexOfThing)
        {
            string json = await messenger.reqestToService(thing[4], thing[5]);
            if (json.Equals("{}")) throw new Exception("there is no data");
            Dictionary<string, string> newData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (!сomparer(newData, indexOfThing)) throw new Exception("repeat data" + json);
            this.loger.writeLog(json);
            this.thingsPropertyInServer[indexOfThing] = newData;
        }
        #region сomparer
        private bool сomparer(Dictionary<string, string> newData, int indexOfThing)
        {
            bool result = true;

            if (this.thingsPropertyInServer[indexOfThing] != null)
            {
                result = checkValues(indexOfThing, newData);
            }

            return result;
        }

        private bool checkValues(int indexOfThing, Dictionary<string, string> newData)
        {
            bool result = false;
            foreach (KeyValuePair<string, string> keyValuePair in newData)
            {
                if (!keyValuePair.Key.Equals("N"))
                {
                    if (!this.thingsPropertyInServer[indexOfThing][keyValuePair.Key].Equals(keyValuePair.Value)) result = true;
                }
            }
            return result;
        }
        #endregion

        private void sendUDP(string[] thing, int indexOfThing)
        {
            UDPSendler sendler = new UDPSendler(thing[2], thing[3]);
            string sendData = this.json.collectStringData(this.thingsPropertyInServer[indexOfThing], thing[0]);
            sendler.sendTo(sendData);
            this.loger.writeLog("send to " + thing[2] + ":" + thing[3] + "   " + sendData);
        }

        private void trySendPropertyToServer(Messenger messenger, int indexOfThing)
        {
            try
            {
                SendPropertyToServer(messenger, indexOfThing);
            }
            catch
            {
                throw new Exception("Error sendPropertyToServer");
            }
        }

        private void SendPropertyToServer(Messenger messenger, int indexOfThing)
        {
            if (this.thingsPropertyInPolygon[indexOfThing] != null)
            {
                string json = JsonConvert.SerializeObject(this.thingsPropertyInPolygon[indexOfThing]);
                this.loger.writeLog("send data to server: " + teamSettings[indexOfThing][4] + json);
                messenger.reqestToService(teamSettings[indexOfThing][4], teamSettings[indexOfThing][5], json);
            }
        }
    }
}
