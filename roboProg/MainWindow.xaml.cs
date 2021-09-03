using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
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
        private int indexOfSelectedThing;
        private AllThingsJson.Rootobject allThings;
        private Dictionary<string, string>[] thingsPropertyInServer;
        private Dictionary<string, string>[] thingsPropertyInPolygon;
        private List<string[]> teamSettings;
        private RequestJson json;
        private Loger loger;

        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
            this.json = new RequestJson();
            this.loger = Loger.getInstance();
            authorization();
            startListening();
        }

        private void authorization()
        {
            fullingAuthFields(getAuthData());
        }

        private string[] getAuthData()
        {
            FileReader reader = new FileReader();
            string[] authdata = reader.readAuthorizationFile();
            return authdata;
        }

        private void fullingAuthFields(string[] authdata)
        {
            AuthKey.Text = authdata[0];
            IP.Text = authdata[1];
            Port.Text = authdata[2];
            Login.Text = authdata[3];
            Password.Password = authdata[4];
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
                WriteErrorLogBox("Error from start listening proces:" + e.Message);
            }
        }
        #endregion

        #region UDP Listening
        private void cachListening()
        {
            try
            {
                listening();
            }
            catch (Exception e)
            {
                WriteErrorLogBox("resive from poligon ERROR: " + e.Message);
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
            try
            {
                writePropertyFromePoligon(message);
            }
            catch(Exception e)
            {
                WriteErrorLogBox(e.Message);
            }
        }

        private void writePropertyFromePoligon(string[] message)
        {
            if (teamCyclicalRun)
            {
                PoligonDataLog("recieve from poligon: " + message[0] + message[1] + ":" + message[2]);
                preparationDataAndWrite(message[0], message[1], message[2]);
            }
        }

        private string[] dataPreparation(string data)
        {
            data = trimer(data);
            string[] result = data.Split(':');
            return result;
        }

        private string trimer(string s)
        {
            s = s.Trim(' ');
            s = s.Trim('\n');
            s = s.Trim('#');
            s = s.Trim(' ');
            return s;
        }

        private void preparationDataAndWrite(string data, string ip, string port)
        {
            int lenght = this.teamSettings.Count;
            for (int i = 0; i < lenght; i++)
            {
                if (checkThing(i, ip, port))
                {
                    writeProperty(i, data);
                }
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

        #region AllThings
        private async void AllThingsButton_Click(object sender, RoutedEventArgs e)
        {
            ServerDatalog("click 'отправить запрос'");
            if (checkFields())
            {
                Messenger messenger = new Messenger(authInfo(), address(), authorizationType());
                string json = await messenger.getAllThings();
                tryWriteAllThings(json);
            }
        }

        private void tryWriteAllThings(string json)
        {
            writeFromServerLogBox(json);
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
            StartTeamLogBoxWrite("select " + name);
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
                WriteErrorLogBox(exception.Message);
            }
        }

        private void teamInfo(string teamName)
        {
            readTeamInfo(teamName);
            preparePropertyFields();
            startButtonPrepare();
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
            StartTeamLogBoxWrite("start " + TeamStart.Content);
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

        #region data transfer
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
                //fullingPropertyView();
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
                WriteErrorLogBox(e.Message);
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
            writeFromServerLogBox(json);
            this.thingsPropertyInServer[indexOfThing] = newData;
        }

        private bool сomparer(Dictionary<string, string> newData, int index)
        {
            bool result = true;
            Dictionary<string, string> thingsProperty = this.thingsPropertyInServer[index];

            if (thingsProperty != null)
            {
                result = checkValues(thingsProperty, newData);
            }

            return result;
        }

        private bool checkValues(Dictionary<string, string> oldData, Dictionary<string, string> newData)
        {
            bool result = false;
            foreach (KeyValuePair<string, string> keyValuePair in newData)
            {
                if (!keyValuePair.Key.Equals("N"))
                {
                    if (!oldData[keyValuePair.Key].Equals(keyValuePair.Value)) result = true;
                }
            }
            return result;
        }

        private void sendUDP(string[] thing, int indexOfThing)
        {
            UDPSendler sendler = new UDPSendler(thing[2], thing[3]);
            string sendData = this.json.collectStringData(this.thingsPropertyInServer[indexOfThing], thing[0]);
            sendler.sendTo(sendData);
            PoligonDataLog("send to " + thing[2] + ":" + thing[3] + "   " + sendData);
        }

        private void sendPropertyToServer(Messenger messenger, int indexOfThing)
        {
            try
            {
                if (this.thingsPropertyInPolygon[indexOfThing] != null)
                {
                    ServerDatalog("send data to server");
                    string json = JsonConvert.SerializeObject(this.thingsPropertyInPolygon[indexOfThing]);
                    messenger.reqestToService(teamSettings[indexOfThing][4], teamSettings[indexOfThing][5], json);
                }
            }catch
            {
                throw new Exception("Error sendPropertyToServer");
            }
        }

        private int pace()
        {
            if (Pace.Text.Length == 0) return 0;
            else return int.Parse(Pace.Text);
        }
        #endregion

        #region view property
        private void TeamThingsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.indexOfSelectedThing = TeamThingsList.SelectedIndex;
            if (this.teamCyclicalRun)
            {
                fullingPropertyView();
            }
            
        }

        private void fullingPropertyView()
        {
            paramFieldClin();
            showProperty(PropertyInServiceViews, thingsPropertyInServer[indexOfSelectedThing]);
            showProperty(PropertyInPolygonViews, thingsPropertyInPolygon[indexOfSelectedThing]);
        }

        private void paramFieldClin()
        {
            SelectedThingName.Text = null;
            PropertyInServiceViews.Document = new FlowDocument();
            PropertyInPolygonViews.Document = new FlowDocument();
        }

        private void showProperty(RichTextBox propertyView, Dictionary<string, string> thingProperty)
        {
            string dataToWrite;
            foreach (KeyValuePair<string, string> property in thingProperty)
            {
                dataToWrite = property.Key + " : " + property.Value + Environment.NewLine;
                propertyView.AppendText(dataToWrite);
            }
        }
        #endregion

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
                WriteErrorLogBox("не указан Ключ!!!");
                error = true;
            }
            else
            {
                StartTeamLogBoxWrite("указанный ключ: " + AuthKey.Text);
            }

            return !error;
        }

        private bool checkLogin()
        {
            bool error = false;
            if (Login.Text.Length == 0)
            {
                WriteErrorLogBox("не указан логин!!!");
                error = true;
            }
            else
            {
                StartTeamLogBoxWrite("указанный логин: " + Login.Text);
            }
            if (Password.Password.Length == 0)
            {
                WriteErrorLogBox("не введен пароль!!!");
                error = true;
            }
            return !error;
        }

        private bool checkAddressField()
        {
            bool error = false;
            if (IP.Text.Equals(""))
            {
                WriteErrorLogBox("не указан IP!!!");
                error = true;
            }
            if (Port.Text.Equals(""))
            {
                WriteErrorLogBox("не указан порт!!!");
                error = true;
            }
            return !error;
        }
        #endregion

        #region log
        private void WriteErrorLogBox(string text)
        {
            ErrorLogBox.AppendText(text + Environment.NewLine);
            ErrorLogBox.ScrollToEnd();
            this.loger.writeLog(text);
        }

        private void StartTeamLogBoxWrite(string text)
        {
            StartTeamLogBox.AppendText(text + Environment.NewLine);
            StartTeamLogBox.ScrollToEnd();
            this.loger.writeLog(text);
        }

        private void ServerDatalog(string text)
        {
            ToServerLogBox.AppendText(text + Environment.NewLine);
            ToServerLogBox.ScrollToEnd();
            //writeLogBox(text);
            this.loger.writeLog(text);
        }

        public void writeFromServerLogBox(string text)
        {
            FromServerLogBox.AppendText(text + Environment.NewLine);
            FromServerLogBox.ScrollToEnd();
        }

        private void PoligonDataLog(string text)
        {
            writeUDPLogBox(text);
            this.loger.writeLog(text);
        }

        public void writeUDPLogBox(string text)
        {
            Dispatcher.Invoke(() =>
            {
                UDPLogBox.AppendText(text + Environment.NewLine);
                UDPLogBox.ScrollToEnd();
            });
        }
        #endregion

        #region initial button
        private void InitializationRecieveButton_Click(object sender, RoutedEventArgs e)
        {
            string[] literals = getLiters();
            foreach(string[] thing in this.teamSettings)
            {
                if (literals.Contains(thing[0].ToLower()))
                {
                    UDPSendler sendler = new UDPSendler(thing[2], thing[3]);
                    sendler.sendTo("r");
                }
            }
        }
        
        private string[] getLiters()
        {
            char[] separators = new char[] { ' ', '.', ',', ':', ';' };
            string[] literals = LiteraOfRecieve.Text.ToLower().Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return literals;
        }
        #endregion

        private void OpenSettingsWindow_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }
    }
}