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
        private Connection connection = Connection.GetConnection();
        public List<Work> taskList = new List<Work>();
        public List<Project> projectList = new List<Project>();

        public MainWindow() {
            InitializeComponent();
            UpdateProjectList();

            CommandBinding commandBinding = new CommandBinding();
            commandBinding.Command = ApplicationCommands.Open;
            commandBinding.Executed += CommandBinding_Executed;

            tbIcon.DoubleClickCommand = ApplicationCommands.Open;
        }

        private void InitilizeConnection() {
            WorkUtility.connection = connection;
            ProjectUtility.connection = connection;
            TeamUtility.connection = connection;
            TaskUtility.connection = connection;
            connection.updateConnection();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
            Show();
            WindowState = prevState;
        }

        public void UpdateProjectList() {
            InitilizeConnection();

            projectList = GetProjectList();
            ProjectList.ItemsSource = projectList;
            ProjectList.DisplayMemberPath = "description"; // возможно, лучше использовать name
            ProjectList.SelectedValuePath = "name";
        }

        private List<Work> GetTaskList(string project) {
            List<ComboData> worksID = WorkUtility.GetWorksId();
            List<Work> worksTitle = WorkUtility.GetWorksTitle(worksID);

            return worksTitle;
        }

        private List<Project> GetProjectList() {
            List<Project> worksTitle = new List<Project>();

            try {
                worksTitle = ProjectUtility.GetProjectTitle();
            } catch (Exception) {
                MessageBox.Show("Проверьте настройки приложения", "Ошибка приложения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return worksTitle;
        }

        private void Window_StateChanged(object sender, EventArgs e) {
            if (WindowState == WindowState.Minimized)
                Hide();
            else {
                prevState = WindowState;
            }
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
                connection.project = project;
                connection.team = TeamUtility.GetProjectTeam();
                connection.area = TaskUtility.GetArea();

                taskList = GetTaskList(project);
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
                TaskUtility.Pause(currentWork, delta);
            }

            currentWork = (int)TaskList.SelectedValue;
            TaskUtility.Play(currentWork);
        }

        private void Pause_Click(object sender, RoutedEventArgs e) {
            var currentDate = DateTime.Now;
            var delta = (currentDate - startDate).TotalSeconds/3600;

            TaskUtility.Pause(currentWork, delta);
        }

        private void Stop_Click(object sender, RoutedEventArgs e) {
            var currentDate = DateTime.Now;
            var delta = (currentDate - startDate).TotalSeconds / 3600;

            TaskUtility.Stop(currentWork, delta);
        }

        private void ShowTray_Click(object sender, RoutedEventArgs e) {
            Show();
            WindowState = prevState;

            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
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
