using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rapscallion {

    static class Program {

        private static string user, pass, authcode, personaname;

        public static bool programRunning = false;

        static void Main(string[] args) {
           // Application.Run(new ClientLogin());

            Roulette.BuildAliasList();

            //Console.ReadKey();

            Console.Write("Username: ");
            user = Console.ReadLine();
            Console.Write("Password: ");
            pass = getPassword();
            Console.WriteLine("\nAttempting login using " + user + " and password " + pass + "...");

            programRunning = true;

            new SteamEvents(user, pass);
            SteamEvents.SetupCallbacks();

            Globals.steamClient.Connect();

            while(Program.programRunning) {
                // in order for the callbacks to get routed, they need to be handled by the manager
                Globals.callbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(0.5));
            }            

            Console.ReadKey();

        }
        private static string getPassword() {

            List<char> pwd = new List<char>();
            string password = "";

            while(true) {

                ConsoleKeyInfo i = Console.ReadKey(true);

                if(i.Key == ConsoleKey.Enter) {

                    break;

                } else if(i.Key == ConsoleKey.Backspace) {

                    if(pwd.Count > 0) {

                        pwd.RemoveAt(pwd.Count - 1);
                        Console.Write("\b \b");

                    }

                } else {

                    pwd.Add(i.KeyChar);
                    Console.Write("*");

                }

            }

            foreach(Char letter in pwd) {
                password += letter;
            }

            return password;
        }
    }
}
