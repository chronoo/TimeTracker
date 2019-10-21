using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using TimeTracker.Projects;
using TimeTracker.Teams;

namespace TimeTracker {
    public partial class MainWindow : Window {
        WindowState prevState;
        public List<Work> taskList = new List<Work>();
        public List<Project> projectList = new List<Project>();
        public MainWindow() {
            InitializeComponent();
            updateProjectList();
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
            var team = TeamUtility.getProjectTeam(organization, project, token);

            List<ComboData> worksID = WorkUtility.getWorksId(organization, project, team, token);
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
            var window = new SettingsWindow();
            window.parentWindow = this;
            window.Show();
        }

        private void ProjectList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            string project = (sender as ComboBox).SelectedValue as string;
            if (project != null) {
                taskList = getTaskList(project);
                TaskList.ItemsSource = taskList;
                TaskList.DisplayMemberPath = "title";
                TaskList.SelectedValue = "id";
            }
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
