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
        public SettingsWindow()
        {
            InitializeComponent();
        }

        #region events
        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NewTabControlItem();
            SettingsRobotTabControl.SelectedIndex = SettingsRobotTabControl.Items.Count - 2;
        }
        #endregion

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
            TextBox textBox = NewTextBox(217);
            textBox.Margin = new Thickness(10, 90, 0, 0);
            textBox.Name = "JsonCharsPoligon";
            return textBox;
        }

        private TextBox CreateNameOfThingBox()
        {
            TextBox textBox = NewTextBox(172);
            textBox.Margin = new Thickness(10, 35, 0, 0);
            textBox.Name = "NameOfThing";
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
            textBox.Name = "Delimetr";
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
    }
}
