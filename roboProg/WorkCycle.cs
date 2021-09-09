using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace roboProg
{
    public class WorkCycle
    {
        private const string _toServer = "to server";
        private const string _fromServer = "from server";
        private const string _toPoligon = "to poligon";
        private const string _fromPoligon = "from poligon";
        private const string _error = "error";

        private MainWindow window;
        private bool Work;
        private int pace;
        private Dispatcher dispatcher;
        private string[] authorizationData = new string[3];
        private bool cyclicalRun = false;
        private Dictionary<string, string>[] thingsPropertyInServer;
        private Dictionary<string, string>[] thingsPropertyInPolygon;
        private List<string[]> teamSettings;
        private Messenger messenger;
        private RequestJson json;
        private Loger loger;

        public int Pace { get => pace; set => pace = value; }

        public void Main(MainWindow window)
        {
            this.window = window;
            this.loger = Loger.getInstance();
            this.json = new RequestJson();
            this.Work = false;
            this.dispatcher = Dispatcher.CurrentDispatcher;
            WorkMethod();
        }

        private void WorkMethod()
        {
            startListening();

            while (Work)
            {
                if (cyclicalRun) CycleMethod();
                Thread.Sleep(5);
            }
        }

        private void startListening()
        {
            Task startListening = new Task(cachListening);
            try
            {
                startListening.Start();
            }
            catch (Exception e)
            {
                sendToWindowTextBox(_error, "Error from start listening proces:" + e.Message);
            }
        }



        #region UDP Listening
        private void cachListening()
        {
            try { listening(); }
            catch (Exception e)
            {
                sendToWindowTextBox(_error, "resive from poligon ERROR: " + e.Message);
                sendToWindowTextBox(_fromPoligon, "Error!!!");
            }
        }

        private void listening()
        {
            UDPReciever listenSocket = UDPReciever.getInstans();
            while (true)
            {
                string[] data = listenSocket.getMessage();
                tryWritePropertyFromePoligon(data);
            }
        }

        private void tryWritePropertyFromePoligon(string[] message)
        {
            try { writePropertyFromePoligon(message); }
            catch (Exception e) { sendToWindowTextBox(_error, e.Message); }
        }

        private void writePropertyFromePoligon(string[] message)
        {
            if (cyclicalRun)
            {
                sendToWindowTextBox(_fromPoligon, message[0] + message[1] + ":" + message[2]);
                preparationDataAndWrite(message[0], message[1], message[2]);
            }
        }

        private void preparationDataAndWrite(string data, string ip, string port)
        {
            int lenght = this.teamSettings.Count;
            for (int i = 0; i < lenght; i++)
            {
                if (checkThing(i, ip, port)) writeProperty(i, data);
            }
        }

        private bool checkThing(int indexOfThing, string ip, string port)
        {
            string[] thing = this.teamSettings[indexOfThing];
            return ip.Equals(thing[2]) && port.Equals(thing[3]);
        }

        private void writeProperty(int indexOfThing, string data)
        {
            ConvertDataToSave convertData = new ConvertDataToSave(this.teamSettings[indexOfThing][0], "poligon");
            this.thingsPropertyInPolygon[indexOfThing] = convertData.getDictionary(data);
        }
        #endregion

        public Dispatcher getDispatcher()
        {
            return this.dispatcher;
        }

        public void TeamInfo(string teamName)
        {
            readTeamInfo(teamName);
            preparePropertyFields();
        }

        private void readTeamInfo(string teamName)
        {
            FileReader reader = new FileReader();
            this.teamSettings = reader.itemInfo(teamName);
        }

        private void preparePropertyFields()
        {
            this.thingsPropertyInServer = new Dictionary<string, string>[teamSettings.Count];
            this.thingsPropertyInPolygon = new Dictionary<string, string>[teamSettings.Count];
        }

        public void SetAuthorizationData(string address, string authInfo, string authorizationType)
        {
            this.authorizationData[0] = address;
            this.authorizationData[1] = authInfo;
            this.authorizationData[2] = authorizationType;
        }

        private void CycleMethod()
        {
            this.messenger = new Messenger(authorizationData[0], authorizationData[1], authorizationData[2]);
            int teamListLenght = this.teamSettings.Count();
            while (cyclicalRun)
            {
                cyclicalRequest(messenger, teamListLenght);
            }
        }

        private void cyclicalRequest(Messenger messenger, int teamListLenght)
        {
            for (int i = 0; i < teamListLenght; i++)
            {
                dataTransfer(messenger, this.teamSettings[i], i);
                if (!this.cyclicalRun) break;
            }
            Thread.Sleep(pace);
        }

        private async void dataTransfer(Messenger messenger, string[] thing, int indexOfThing)
        {
            if (await cachProperty(messenger, thing, indexOfThing))
            {
                sendUDP(thing, indexOfThing);
                trySendPropertyToServer(messenger, indexOfThing);
            }
        }

        private async Task<bool> cachProperty(Messenger messenger, string[] thing, int indexOfThing)
        {
            bool result = true;
            try { await property(messenger, thing, indexOfThing); }
            catch (Exception e)
            {
                sendToWindowTextBox(_error, e.Message);
                sendToWindowTextBox(_fromServer, "Error!!!");
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
            sendToWindowTextBox(_fromServer, json);
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
            sendToWindowTextBox(_toPoligon, thing[2] + ":" + thing[3] + "   " + sendData);
        }

        private void trySendPropertyToServer(Messenger messenger, int indexOfThing)
        {
            try { SendPropertyToServer(messenger, indexOfThing); }
            catch { throw new Exception("Error sendPropertyToServer"); }
        }

        private async void SendPropertyToServer(Messenger messenger, int indexOfThing)
        {
            if (this.thingsPropertyInPolygon[indexOfThing] != null)
            {
                string json = JsonConvert.SerializeObject(this.thingsPropertyInPolygon[indexOfThing]);
                sendToWindowTextBox(_toServer, teamSettings[indexOfThing][4] + json);
                await messenger.reqestToService(teamSettings[indexOfThing][4], teamSettings[indexOfThing][5], json);
            }
        }

        private void sendToWindowTextBox(string box, string text)
        {
            loger.writeLog(box + ": " + text);
            window.ShowInTextBox(box, text);
        }

        public void StartCycle()
        {
            this.cyclicalRun = true;
        }

        public void StopCycle()
        {
            this.cyclicalRun = false;
        }

        public void Close()
        {
            this.cyclicalRun = false;
            this.Work = false;
        }
    }
}
