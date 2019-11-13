using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Teams;

namespace TimeTracker {
    public class Connection {
        public string organization { get; set; }
        public string project { get; set; }
        public string token { get; set; }
        public string eMail { get; set; }
        public string team { get; set; }
        public string area { get; set; }

        private Connection() { }

        private static Connection connection;
        public static Connection GetConnection() {
            if(connection == null) {
                connection = new Connection();
            }
            return connection;
        }

        public void updateConnection() {
            var connection = GetConnection();
            connection.organization = Properties.Settings.Default.Organization;
            connection.token = Properties.Settings.Default.Token;
            connection.eMail = Properties.Settings.Default.EMail;
        }

    }
}
