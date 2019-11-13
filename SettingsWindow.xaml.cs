using System.Windows;

namespace TimeTracker
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public MainWindow parentWindow;
        public SettingsWindow()
        {
            InitializeComponent();
            Organization.Text = Properties.Settings.Default.Organization;
            Token.Text = Properties.Settings.Default.Token;
            EMail.Text = Properties.Settings.Default.EMail;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Organization = Organization.Text;
            Properties.Settings.Default.Token = Token.Text;
            Properties.Settings.Default.EMail = EMail.Text;

            Properties.Settings.Default.Save();

            parentWindow.UpdateProjectList(); // надо переделать под оповещение формы

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
