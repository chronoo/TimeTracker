﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Forms;

namespace TimeTracker
{
    public partial class MainWindow : Window
    {
        WindowState prevState;
        public List<Work> ListData = new List<Work>();
        public MainWindow()
        {
            InitializeComponent();

            ListData = getTaskList();
            TaskList.ItemsSource = ListData;

            TaskList.DisplayMemberPath = "title";
            TaskList.SelectedValue = "id";
        }

        private List<Work> getTaskList()
        {
            var organization    = ConfigurationManager.AppSettings["organization"];
            var project         = ConfigurationManager.AppSettings["project"];
            var team            = ConfigurationManager.AppSettings["team"];
            var Base64Token     = ConfigurationManager.AppSettings["Base64Token"];

            List<ComboData> worksID = WorksUtility.getWorksId(organization, project, team, Base64Token);
            List<Work> worksTitle = WorksUtility.getWorksTitle(organization, project, Base64Token, worksID);
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
