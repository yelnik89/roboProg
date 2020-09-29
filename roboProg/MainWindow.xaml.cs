using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace roboProg
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool teamCyclicalRun = false;
        private string teamName;
        private AllThingsJson.Rootobject allThings;
        private Dictionary<string, string>[] thingsPropertyInServer;
        private Dictionary<string, string>[] thingsPropertyInPolygon;
        private List<string[]> teamSettings;
        private RequestJson json;

        public MainWindow()
        {
            InitializeComponent();
            this.json = new RequestJson();

            Task startListening = new Task(cachListening);
            try
            {
                startListening.Start();
            }
            catch (Exception e)
            {
                log("resive from poligon ERROR:" + e.Message);
            }
        }

        #region UDP Listening
        private void cachListening()
        {
            try
            {
                listening();
            }
            catch (Exception e)
            {
                log("resive from poligon ERROR: " + e.Message);
            }
        }

        private void listening()
        {
            UDPReciver listenSocket = UDPReciver.getInstans();
            while (true)
            {
                string[] data = listenSocket.getMessage();
                writePropertyFromePoligon(data);
            }
        }

        private void writePropertyFromePoligon(string[] message)
        {
            if (teamCyclicalRun)
            {
                writeUDPLog(message);
                string[] data = dataPreparation(message[0]);
                preparationDataAndWrite(data, message[1], message[2]);
            }
        }

        private string[] dataPreparation(string data)
        {
            data = trimer(data);
            string[] result = data.Split(':');
            return result;
        }

        private void preparationDataAndWrite(string[] data, string ip, string port)
        {
            int lenght = this.teamSettings.Count;
            for (int i = 0; i < lenght; i++)
            {
                string[] thing = this.teamSettings[i];
                if (ip.Equals(thing[2]) && port.Equals(thing[3]))
                {
                    writeProperty(i, data);
                }
            }
        }

        private void writeProperty(int indexOfThing, string[] data)
        {
            this.thingsPropertyInPolygon[indexOfThing] = new Dictionary<string, string>();
            string[] template = this.json.getTemplateFromPoligon(data[0]);
            for (int i = 2; i < data.Length; i++)
            {
                this.thingsPropertyInPolygon[indexOfThing].Add(template[i - 2], data[i]);
            }
        }

        private string trimer(string s)
        {
            s = s.Trim(' ');
            s = s.Trim('\n');
            s = s.Trim('#');
            s = s.Trim(' ');
            return s;
        }

        private void writeUDPLog(string[] data)
        {
            log("recieve from poligon: " + data[0] + "\n" +
                       data[1] + ":" + data[2]);
        }
        #endregion

        #region AllThings
        private async void AllThingsButton_Click(object sender, RoutedEventArgs e)
        {
            log("click 'отправить запрос'");
            if (checkFields())
            {
                Messenger messenger = new Messenger(authInfo(), address(), authorizationType());
                string json = await messenger.getAllThings();
                tryWriteAllThings(json);
            }
        }

        private void tryWriteAllThings(string json)
        {
            writeLogBox(json);
            try
            {
                writeAllThing(json);
            }
            catch { }
        }

        private void writeAllThing(string json)
        {
            this.allThings = JsonConvert.DeserializeObject<AllThingsJson.Rootobject>(json);
            if (this.allThings != null)
            {
                this.thingsPropertyInServer = new Dictionary<string, string>[this.allThings.rows.Length];
                fullingThingList(this.allThings);
            }
        }

        private void fullingThingList(AllThingsJson.Rootobject allThings)
        {
            foreach (AllThingsJson.Row row in allThings.rows)
            {
                AllThingsList.Items.Add(row.name);
            }

            AllThingsList.SelectedIndex = 0;
        }
        #endregion

        #region Team functions
        private void Team1_Click(object sender, RoutedEventArgs e)
        {
            teamClick("Team1");
        }

        private void teamClick(string name)
        {
            log("select " + name);
            сachItemInfo(name);
        }

        private void сachItemInfo(string teamName)
        {
            try
            {
                teamInfo(teamName);
                fullingTeamThingsList(this.teamSettings);
            }
            catch (Exception exception)
            {
                log(exception.Message);
            }
        }

        private void teamInfo(string teamName)
        {
            readTeamInfo(teamName);
            startButtonPrepare();
            preparePropertyFields();
        }

        private void readTeamInfo(string teamName)
        {
            FileReader reader = new FileReader();
            this.teamSettings = reader.itemInfo(teamName);
            this.teamName = teamName;
        }

        private void startButtonPrepare()
        {
            TeamStart.Content = teamName;
            TeamStart.IsEnabled = true;
        }

        private void preparePropertyFields()
        {
            this.thingsPropertyInServer = new Dictionary<string, string>[teamSettings.Count];
            this.thingsPropertyInPolygon = new Dictionary<string, string>[teamSettings.Count];
        }

        private void fullingTeamThingsList(List<string[]> teamList)
        {
            int count = teamList.Count;
            for (int i = 0; i < count; i++)
            {
                string[] thing = teamList[i];
                this.TeamThingsList.Items.Add(thing[4]);
            }
            TeamThingsList.SelectedIndex = 0;
        }
        #endregion

        #region cyclical functions
        private void TeamStart_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                if (this.teamCyclicalRun) stopTeamCicleRequest();
                else startTeamCicleRequest();
            }
        }

        private void startTeamCicleRequest()
        {
            TeamStart.Content = "STOP";
            this.teamCyclicalRun = true;
            cyclicalMethod();
        }

        private void stopTeamCicleRequest()
        {
            this.teamCyclicalRun = false;
            TeamStart.Content = this.teamName;
        }

        private async void cyclicalMethod()
        {
            Messenger messenger = new Messenger(authInfo(), address(), authorizationType());
            int teamListLenght = this.teamSettings.Count();
            while (this.teamCyclicalRun)
            {
                await cyclicalRequest(messenger, teamListLenght);
            }
        }

        private async Task cyclicalRequest(Messenger messenger, int teamListLenght)
        {
            for (int i = 0; i < teamListLenght; i++)
            {
                await dataTransfer(messenger, this.teamSettings[i], i);
                if (!this.teamCyclicalRun) break;
            }
            await Task.Delay(pace());
        }

        private async Task dataTransfer(Messenger messenger, string[] thing, int indexOfThing)
        {
            if (await cachProperty(messenger, thing, indexOfThing))
            {
                sendUDP(thing, indexOfThing);
                sendPropertyToServer(messenger, indexOfThing);
            }
        }

        private async Task<bool> cachProperty(Messenger messenger, string[] thing, int indexOfThing)
        {
            bool result = true;
            try
            {
                await property(messenger, thing, indexOfThing);
            }
            catch(Exception e)
            {
                writeLogBox(e.Message);
                result = false;
            }

            return result;
        }

        private async Task property(Messenger messenger, string[] thing, int indexOfThing)
        {
            string json = await messenger.reqestToService(thing[4], thing[5]);
            writeLogBox(json);
            this.thingsPropertyInServer[indexOfThing] = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        private void sendUDP(string[] thing, int indexOfThing)
        {
            UDPSendler sendler = new UDPSendler(thing[2], thing[3]);
            string sendData = this.json.collectStringData(this.thingsPropertyInServer[indexOfThing], thing[0]);
            sendler.sendTo(sendData);
        }

        private void sendPropertyToServer(Messenger messenger, int indexOfThing)
        {
            if (this.thingsPropertyInPolygon[indexOfThing] != null)
            {
                log("send data to server");
                string json = JsonConvert.SerializeObject(this.thingsPropertyInPolygon[indexOfThing]);
                messenger.reqestToService(teamSettings[indexOfThing][4], teamSettings[indexOfThing][5], json);
            }
        }

        private int pace()
        {
            if (Pace.Text.Length == 0) return 0;
            else return int.Parse(Pace.Text);
        }
        #endregion

        #region address
        private string address()
        {
            if (checkAddressField())
            {
                return compilAdderss();
            }
            return "";
        }

        private string compilAdderss()
        {
            return "http://" + IP.Text + ":" + Port.Text;
        }
        #endregion

        #region authorization
        private string authInfo()
        {
            string authInfo = "";
            if (AuthType.SelectedIndex == 1)
            {
                authInfo = Login.Text + ":" + Password.Password;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            }
            if (AuthType.SelectedIndex == 0) authInfo = AuthKey.Text;
            return authInfo;
        }

        private string authorizationType()
        {
            string type = "";
            if (AuthType.SelectedIndex == 1) type = "Basic";
            if (AuthType.SelectedIndex == 0) type = "appkey";
            return type;
        }
        #endregion

        #region checkFields
        private bool checkFields()
        {
            bool error = false;
            if (!checkAuthorizationField()) error = true;
            if (!checkAddressField()) error = true;
            return !error;
        }

        private bool checkAuthorizationField()
        {
            bool error = false;
            if (AuthType.SelectedIndex == 0)
            {
                if (!checkAuthKey()) error = true;
            }
            else if (AuthType.SelectedIndex == 1)
            {
                if (checkLogin()) error = true;
            }
            return !error;
        }

        private bool checkAuthKey()
        {
            bool error = false;
            if (AuthKey.Text.Length == 0)
            {
                log("не указан Ключ!!!");
                error = true;
            }
            else
            {
                log("указанный ключ: " + AuthKey.Text);
            }

            return !error;
        }

        private bool checkLogin()
        {
            bool error = false;
            if (Login.Text.Length == 0)
            {
                log("не указан логин!!!");
                error = true;
            }
            else
            {
                log("указанный логин: " + Login.Text);
            }
            if (Password.Password.Length == 0)
            {
                log("не введен пароль!!!");
                error = true;
            }
            return !error;
        }

        private bool checkAddressField()
        {
            bool error = false;
            if (IP.Text.Equals(""))
            {
                log("не указан IP!!!");
                error = true;
            }
            if (Port.Text.Equals(""))
            {
                log("не указан порт!!!");
                error = true;
            }
            return !error;
        }
        #endregion

        #region log
        private void log(string text)
        {
            Loger loger = Loger.getInstance();
            writeLogBox(text);
            loger.writeLog(text);
        }

        public void writeLogBox(string text)
        {
            Dispatcher.Invoke(() => 
            {
                LogBox.AppendText(text + Environment.NewLine);
                LogBox.ScrollToEnd();
            });
        }
        #endregion
    }
}