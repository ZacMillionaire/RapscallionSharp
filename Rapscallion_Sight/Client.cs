using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rapscallion {
    public partial class ClientLogin : Form {

        private static string user, pass, authcode, personaname;

        public ClientLogin() {
            InitializeComponent();
            /*
            // create our callback handling loop
            while(Program.programRunning) {
                // in order for the callbacks to get routed, they need to be handled by the manager
                Globals.callbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(0.5));
            }*/

        }

        private void button1_Click(object sender, EventArgs e) {

            user = input_username.Text;
            pass = input_password.Text;

            Program.programRunning = true;

            new SteamEvents(user, pass);
            SteamEvents.SetupCallbacks();

            Globals.steamClient.Connect();
        }
    }
}
