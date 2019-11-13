using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimeTracker.Projects;
using TimeTracker.Tasks;
using TimeTracker.Teams;

namespace TimeTracker {
    public partial class MainWindow : Window {
        private const int NULL = -1;
        private WindowState prevState;
        private DateTime startDate;
        private int currentWork = NULL;
        private string currentProject;
        private string organization = Properties.Settings.Default.Organization;
        private string token = Properties.Settings.Default.Token;

        public List<Work> taskList = new List<Work>();
        public List<Project> projectList = new List<Project>();
        public MainWindow() {
            InitializeComponent();
            updateProjectList();

            CommandBinding commandBinding = new CommandBinding();
            commandBinding.Command = ApplicationCommands.Open;
            commandBinding.Executed += CommandBinding_Executed;

            tbIcon.DoubleClickCommand = ApplicationCommands.Open;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
            Show();
            WindowState = prevState;
        }

        public void updateProjectList() {
            projectList = getProjectList();
            ProjectList.ItemsSource = projectList;
            ProjectList.DisplayMemberPath = "description"; // возможно, лучше использовать name
            ProjectList.SelectedValuePath = "name";
        }

        private List<Work> getTaskList(string project) {
            var organization = Properties.Settings.Default.Organization;
            var token = Properties.Settings.Default.Token;
            var eMail = Properties.Settings.Default.EMail;
            var team = TeamUtility.getProjectTeam(organization, project, token);

            List<ComboData> worksID = WorkUtility.getWorksId(organization, project, team, token, eMail);
            List<Work> worksTitle = WorkUtility.getWorksTitle(organization, project, token, worksID);

            return worksTitle;
        }

        private List<Project> getProjectList() {
            List<Project> worksTitle = new List<Project>();
            var organization = Properties.Settings.Default.Organization;
            var token = Properties.Settings.Default.Token;

            try {
                worksTitle = ProjectUtility.getProjectTitle(organization, token);
            } catch (Exception) {
                MessageBox.Show("Проверьте настройки приложения", "Ошибка приложения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return worksTitle;
        }

        private void Window_StateChanged(object sender, EventArgs e) {
            if (WindowState == WindowState.Minimized)
                Hide();
            else
                prevState = WindowState;
        }

        private void TaskbarIcon_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Show();
            WindowState = prevState;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            var window = new SettingsWindow {
                parentWindow = this
            };
            window.Show();
        }

        private void ProjectList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            string project = (sender as ComboBox).SelectedValue as string;
            if (project != null) {
                currentProject = project;

                taskList = getTaskList(project);
                TaskList.ItemsSource = taskList;
                TaskList.DisplayMemberPath = "title";
                TaskList.SelectedValuePath = "id";
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e) {
            var currentDate = DateTime.Now;
            var delta = (currentDate - startDate).TotalSeconds / 3600;

            startDate = DateTime.Now;
            if (currentWork != NULL && currentWork != (int)TaskList.SelectedValue) { 
                TaskUtility.pause(currentWork, organization, currentProject, token, delta);
            }

            currentWork = (int)TaskList.SelectedValue;
            TaskUtility.play(currentWork, organization, currentProject, token);
        }

        private void Pause_Click(object sender, RoutedEventArgs e) {
            var currentDate = DateTime.Now;
            var delta = (currentDate - startDate).TotalSeconds/3600;

            TaskUtility.pause(currentWork, organization, currentProject, token, delta);
        }

        private void Stop_Click(object sender, RoutedEventArgs e) {
            var currentDate = DateTime.Now;
            var delta = (currentDate - startDate).TotalSeconds / 3600;

            TaskUtility.stop(currentWork, organization, currentProject, token, delta);
        }

        private void showTray_Click(object sender, RoutedEventArgs e) {
            Show();
            WindowState = prevState;
        }
    }
    public class ComboData {
        public ComboData(int Id, string Value) {
            this.Id = Id;
            this.Value = Value;
        }
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
