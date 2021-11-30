using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace roboProg
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly char[] _separators = { ' ', '.', ',', ':', ';', '-', '_', '!', '?', '*', '#', '(', ')', '"' };

        public SettingsWindow()
        {
            InitializeComponent();
            if (SETTINGS.RobotSettings != null && SETTINGS.RobotSettings.Count != 0)
                ViewRobotSettings();
            else
                NewTabControlItem();
        }

        #region events
        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NewTabControlItem();
            SettingsRobotTabControl.SelectedIndex = SettingsRobotTabControl.Items.Count - 2;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TrySave();
        }

        private void SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            TrySave();
            this.Close();
        }
        #endregion

        private void ViewRobotSettings()
        {
            RobotSettings[] robots = GetSettingsArray();
            CreateTabs(robots.Count());
            fullingTabs(robots);
        }

        private RobotSettings[] GetSettingsArray()
        {
            int i = 0;
            RobotSettings[] robots = new RobotSettings[SETTINGS.RobotSettings.Count];

            foreach (KeyValuePair<string, RobotSettings> pair in SETTINGS.RobotSettings)
            {
                robots[i] = pair.Value;
                i++;
            }

            return robots;
        }

        private void CreateTabs(int count)
        {
            for (int i = 0; i < count; i++)
                NewTabControlItem();
        }

        private void fullingTabs(RobotSettings[] robots)
        {
            for(int i = 0; i < robots.Count(); i++)
            {
                WriteSettingsInFields((TabItem)SettingsRobotTabControl.Items[i], robots[i]);
            }
        }

        private void WriteSettingsInFields(TabItem tab, RobotSettings robot)
        {
            tab.Header = robot.Name;
            Grid grid = (Grid)tab.Content;
            TextBox keys = (TextBox)grid.Children[0];
            keys.Text = robot.DataTemplateString();
            TextBox name = (TextBox)grid.Children[1];
            name.Text = robot.Name;
            TextBox litera = (TextBox)grid.Children[2];
            litera.Text = robot.Litera;
            TextBox separator = (TextBox)grid.Children[3];
            separator.Text = robot.Separator;
        }

        private void NewTabControlItem()
        {
            TabItem tabItem = new TabItem
            {
                Header = new TextBlock { Text = "New robot" },
                Content = NewSettingsGrid()
            };
            SettingsRobotTabControl.Items.Insert(SettingsRobotTabControl.Items.Count - 1, tabItem);
        }

        private Grid NewSettingsGrid()
        {
            Grid grid = new Grid();
            grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5));
            FullingGrid(grid);
            return grid;
        }

        private void FullingGrid(Grid grid)
        {
            AddTextBoxes(grid);
            AddLabels(grid);
        }

        #region TextBoxes
        private void AddTextBoxes(Grid grid)
        {
            grid.Children.Add(CreateJsonCharsPoligonBox());
            grid.Children.Add(CreateNameOfThingBox());
            grid.Children.Add(CreateLiteraBox());
            grid.Children.Add(CreateDelimetrBox());
        }

        private TextBox CreateJsonCharsPoligonBox()
        {
            TextBox textBox = NewTextBox(217, 46);
            textBox.Margin = new Thickness(10, 90, 0, 0);
            textBox.Name = "Keys";
            return textBox;
        }

        private TextBox CreateNameOfThingBox()
        {
            TextBox textBox = NewTextBox(172);
            textBox.Margin = new Thickness(10, 35, 0, 0);
            textBox.Name = "Name";
            return textBox;
        }

        private TextBox CreateLiteraBox()
        {
            TextBox textBox = NewTextBox(30);
            textBox.Margin = new Thickness(277, 35, 0, 0);
            textBox.Name = "Litera";
            return textBox;
        }

        private TextBox CreateDelimetrBox()
        {
            TextBox textBox = NewTextBox(30);
            textBox.Margin = new Thickness(277, 90, 0, 0);
            textBox.Name = "Separator";
            return textBox;
        }

        private TextBox NewTextBox(int width, int height = 23)
        {
            TextBox textBox = new TextBox();
            textBox.Height = height;
            textBox.Width = width;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.TextWrapping = TextWrapping.Wrap;
            return textBox;
        }
        #endregion

        #region Labels
        private void AddLabels(Grid grid)
        {
            grid.Children.Add(CreateRobotNameLabel());
            grid.Children.Add(CreateLitera());
            grid.Children.Add(CreateJsonCharsPoligonLabel());
            grid.Children.Add(CreateSeparatorLabel());
        }

        private Label CreateRobotNameLabel()
        {
            Label label = NewLabel();
            label.Margin = new Thickness(10, 9, 0, 0);
            label.Content = "Robot name";
            return label;
        }

        private Label CreateLitera()
        {
            Label label = NewLabel();
            label.Margin = new Thickness(268, 9, 0, 0);
            label.Content = "Litera";
            return label;
        }

        private Label CreateJsonCharsPoligonLabel()
        {
            Label label = NewLabel();
            label.Margin = new Thickness(10, 64, 0, 0);
            label.Content = "Keys";
            return label;
        }

        private Label CreateSeparatorLabel()
        {
            Label label = NewLabel();
            label.Margin = new Thickness(261, 64, 0, 0);
            label.Content = "Separator";
            return label;
        }

        private Label NewLabel()
        {
            Label label = new Label();
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;
            return label;
        }
        #endregion


        private void TrySave()
        {
            try
            {
                SaveData();
            }
            catch (Exception error)
            {
                MessageBox.Show("field " + error.Message + " is empty");
            }
        }

        private void SaveData()
        {
            foreach (TabItem tabItem in SettingsRobotTabControl.Items)
            {
                if (tabItem.Content == null) continue;
                SETTINGS.SetRobotSettings(GetRobotSettings(tabItem));
            }
            FileWriter writer = new FileWriter();
            writer.SaveRobotSettings();
        }

        private RobotSettings GetRobotSettings(TabItem tabItem)
        {
            string[] settings = new string[4];

            for (int i = 0; i < 4; i++)
            {
                settings[i] = GetStringInTextBox(tabItem, i);
            }

            return new RobotSettings(settings[1], settings[2], settings[3], getLiters(settings[0]));
        }

        private string GetStringInTextBox(TabItem tabItem, int index)
        {
            Grid grid = (Grid)tabItem.Content;
            TextBox charsBox = (TextBox)grid.Children[index];
            string result = charsBox.Text;
            if (result.Equals("")) throw new Exception(charsBox.Name);
            return result;
        }

        private string[] getLiters(string str)
        {
            string[] literals = str.ToLower().Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            return literals;
        }
    }
}
