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
using System.Threading;

namespace roboProg
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _pace = 1000;
        private bool teamCyclicalRun = false;
        private string teamName;
        private int indexOfSelectedThing;
        private AllThingsJson.Rootobject allThings;
        private Dictionary<string, string>[] thingsPropertyInServer;
        private Dictionary<string, string>[] thingsPropertyInPolygon;
        private List<string[]> teamSettings;
        private RequestJson json;
        private Dispatcher workDispatcher;
        private Loger loger;
        public WorkCycle Work;

        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
            this.json = new RequestJson();
            this.loger = Loger.getInstance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            authorization();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Work = new WorkCycle();
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Work.Main(this);
            }));
            thread.Start();
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
                Work.TeamInfo(teamName);
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
            Work.StartCycle();
        }

        private void stopTeamCicleRequest()
        {
            this.teamCyclicalRun = false;
            Work.StopCycle();
            TeamStart.Content = this.teamName;
        }

        private int pace()
        {
            
            if (Pace.Text.Length == 0) Pace.Text = Convert.ToString(_pace);
            else
            {
                try { _pace = Convert.ToInt32(Pace.Text); }
                catch { Pace.Text = Convert.ToString(_pace); }
            }
            
            return _pace;

        }

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
                ToPoligonLogBox.AppendText(text + Environment.NewLine);
                ToPoligonLogBox.ScrollToEnd();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Work.Close();
        }

        private void Pace_LostFocus(object sender, RoutedEventArgs e)
        {
            Work.Pace = pace();
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Work.Pace = pace();
        }

        #region Window Interfase
        public void ShowInTextBox(string box, string text)
        {
            switch (box)
            {
                case "error":
                    showError(text);
                    break;
                case "to server":
                    showDataToSever(text);
                    break;
                case "from server":
                    showDataFromServer(text);
                    break;
                case "to poligon":
                    showDataFromServer(text);
                    break;
                case "from poligon":
                    showDataFromServer(text);
                    break;
            }
        }

        private void showError(string text)
        {
            writeTextOnTextBox(ErrorLogBox, text);
        }

        private void showDataToSever(string text)
        {
            writeTextOnTextBox(ToServerLogBox, text);
        }

        private void showDataFromServer(string text)
        {
            writeTextOnTextBox(FromServerLogBox, text);
        }
        private void showDataToPoligon(string text)
        {
            writeTextOnTextBox(ToPoligonLogBox, text);
        }
        private void showDataFromPoligon(string text)
        {
            writeTextOnTextBox(FromPoligonLogBox, text);
        }

        private void writeTextOnTextBox(RichTextBox textBox, string text)
        {
            Dispatcher.Invoke(() =>
            {
                textBox.AppendText(text + Environment.NewLine);
                textBox.ScrollToEnd();
            });
        }
        #endregion
    }
}