using System.Windows;

namespace TimeTracker
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Organization.Text = Properties.Settings.Default.Organization;
            Token.Text = Properties.Settings.Default.Token;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Organization = Organization.Text;
            Properties.Settings.Default.Token = Token.Text;
            Properties.Settings.Default.Save();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
