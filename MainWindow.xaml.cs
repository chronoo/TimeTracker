using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Forms;
using TimeTracker.Projects;

namespace TimeTracker
{
    public partial class MainWindow : Window
    {
        WindowState prevState;
        public List<Work> taskList = new List<Work>();
        public List<Project> projectList = new List<Project>();
        public MainWindow()
        {
            InitializeComponent();

            taskList = getTaskList();
            TaskList.ItemsSource = taskList;
            TaskList.DisplayMemberPath = "title";
            TaskList.SelectedValue = "id";

            projectList = getProjectList();
            ProjectList.ItemsSource = projectList;          
            ProjectList.DisplayMemberPath = "description"; // возможно, лучше использовать name
            ProjectList.SelectedValuePath = "name";
        }

        private List<Work> getTaskList()
        {
            var organization    = ConfigurationManager.AppSettings["organization"];
            var project         = ConfigurationManager.AppSettings["project"];
            var team            = ConfigurationManager.AppSettings["team"];
            var Base64Token     = ConfigurationManager.AppSettings["Base64Token"];

            List<ComboData> worksID = WorkUtility.getWorksId(organization, project, team, Base64Token);
            List<Work> worksTitle = WorkUtility.getWorksTitle(organization, project, Base64Token, worksID);
            return worksTitle;
        }

        private List<Project> getProjectList()
        {
            var organization    = ConfigurationManager.AppSettings["organization"];
            var project         = ConfigurationManager.AppSettings["project"];
            var team            = ConfigurationManager.AppSettings["team"];
            var Base64Token     = ConfigurationManager.AppSettings["Base64Token"];

            List<Project> worksTitle = ProjectUtility.getProjectTitle(organization, Base64Token);
            return worksTitle;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            else
                prevState = WindowState;
        }

        private void TaskbarIcon_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Show();
            WindowState = prevState;
        }
    }
    public class ComboData
    {
        public ComboData(int Id, string Value)
        {
            this.Id = Id;
            this.Value = Value;
        }
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
