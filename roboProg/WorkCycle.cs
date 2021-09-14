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

        private MainWindow _window;
        private bool _work;
        private int _pace;
        private Dispatcher _dispatcher;
        private string[] _authorizationData = new string[3];
        private bool _cyclicalRun = false;
        private Dictionary<string, string>[] _thingsPropertyInServer;
        private Dictionary<string, string>[] _thingsPropertyInPolygon;
        private List<string[]> _teamSettings;
        private Messenger _messenger;
        private JsonTemplate _json;
        private Loger _loger;

        public int Pace { get => _pace; set => _pace = value; }

        public void Main(MainWindow window)
        {
            _window = window;
            _loger = Loger.getInstance();
            _json = new JsonTemplate();
            _work = true;
            _dispatcher = Dispatcher.CurrentDispatcher;
            WorkMethod();
        }

        public void SetAuthorizationData(string authInfo, string address, string authorizationType)
        {
            _authorizationData[0] = authInfo;
            _authorizationData[1] = address;
            _authorizationData[2] = authorizationType;
        }

        private void WorkMethod()
        {
            startListening();

            while (_work)
            {
                if (_cyclicalRun) CycleMethod();
                Thread.Sleep(3);
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
            if (_cyclicalRun)
            {
                preparationDataAndWrite(message[0], message[1], message[2]);
                sendToWindowTextBox(_fromPoligon, message[0] + message[1] + ":" + message[2]);
            }
        }

        private void preparationDataAndWrite(string data, string ip, string port)
        {
            int lenght = _teamSettings.Count;
            for (int i = 0; i < lenght; i++)
            {
                if (checkThing(i, ip, port)) writeProperty(i, data);
            }
        }

        private bool checkThing(int indexOfThing, string ip, string port)
        {
            string[] thing = _teamSettings[indexOfThing];
            return ip.Equals(thing[2]) && port.Equals(thing[3]);
        }

        private void writeProperty(int indexOfThing, string data)
        {
            ConvertDataToSave convertData = new ConvertDataToSave(_teamSettings[indexOfThing][0], "poligon");
            _thingsPropertyInPolygon[indexOfThing] = convertData.getDictionary(data);
        }
        #endregion

        public Dispatcher getDispatcher()
        {
            return _dispatcher;
        }

        public void TeamInfo(string teamName)
        {
            readTeamInfo(teamName);
            preparePropertyFields();
        }

        private void readTeamInfo(string teamName)
        {
            FileReader reader = new FileReader();
            _teamSettings = reader.itemInfo(teamName);
        }

        private void preparePropertyFields()
        {
            _thingsPropertyInServer = new Dictionary<string, string>[_teamSettings.Count];
            _thingsPropertyInPolygon = new Dictionary<string, string>[_teamSettings.Count];
        }

        private void CycleMethod()
        {
            _messenger = new Messenger(_authorizationData[0], _authorizationData[1], _authorizationData[2]);
            int teamListLenght = _teamSettings.Count();
            while (_cyclicalRun)
            {
                cyclicalRequest(_messenger, teamListLenght);
            }
        }

        private void cyclicalRequest(Messenger messenger, int teamListLenght)
        {
            for (int i = 0; i < teamListLenght; i++)
            {
                dataTransfer(messenger, _teamSettings[i], i);
                if (!_cyclicalRun) break;
            }
            Thread.Sleep(_pace);
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
            _thingsPropertyInServer[indexOfThing] = newData;
        }

        #region сomparer
        private bool сomparer(Dictionary<string, string> newData, int indexOfThing)
        {
            bool result = true;

            if (_thingsPropertyInServer[indexOfThing] != null)
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
                    if (!_thingsPropertyInServer[indexOfThing][keyValuePair.Key].Equals(keyValuePair.Value)) result = true;
                }
            }
            return result;
        }
        #endregion

        private void sendUDP(string[] thing, int indexOfThing)
        {
            UDPSendler sendler = new UDPSendler(thing[2], thing[3]);
            string sendData = _json.collectStringData(_thingsPropertyInServer[indexOfThing], thing[0]);
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
            if (_thingsPropertyInPolygon[indexOfThing] != null)
            {
                string json = JsonConvert.SerializeObject(_thingsPropertyInPolygon[indexOfThing]);
                sendToWindowTextBox(_toServer, _teamSettings[indexOfThing][4] + json);
                await messenger.reqestToService(_teamSettings[indexOfThing][4], _teamSettings[indexOfThing][5], json);
            }
        }

        private void sendToWindowTextBox(string box, string text)
        {
            _loger.writeLog(box + ": " + text);
            _window.ShowInTextBox(box, text);
            Thread.Sleep(1);
        }

        public void StartCycle()
        {
            _cyclicalRun = true;
        }

        public void StopCycle()
        {
            _cyclicalRun = false;
        }

        public void Close()
        {
            _cyclicalRun = false;
            _work = false;
        }
    }
}
