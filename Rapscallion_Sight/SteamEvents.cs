using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Rapscallion {
    class SteamEvents {

        public static Callback<SteamClient.ConnectedCallback> ClientConnection;
        public static Callback<SteamUser.LoggedOnCallback> ClientLoggedOn;
        public static Callback<SteamClient.DisconnectedCallback> ClientDisconnection;
        public static Callback<SteamUser.LoggedOffCallback> ClientLoggedOff;
        public static Callback<SteamUser.AccountInfoCallback> ClientAccountInforationReceived;
        public static JobCallback<SteamUser.UpdateMachineAuthCallback> JobUpdateAuthDetails;
        public static Callback<SteamFriends.ChatMsgCallback> ClientChatMessage;
        public static Callback<SteamFriends.FriendMsgCallback> ClientFriendMessage;
        public static Callback<SteamFriends.ChatInviteCallback> ClientChatInvite;
        public static Callback<SteamFriends.ChatMemberInfoCallback> ClientChatStatusChanged;
        public static Callback<SteamFriends.ChatEnterCallback> ClientChatEntered;

        private static string user, pass, authcode, personaname;

        public SteamEvents() {

        }

        public SteamEvents(string username, string password) {

            user = username;
            pass = password;

        }
        public SteamEvents(string username, string password, string authCode) {

            user = username;
            pass = password;
            authcode = authCode;

        }
        public static void SetupCallbacks() {

            Globals.steamUser = Globals.steamClient.GetHandler<SteamUser>();
            Globals.steamFriends = Globals.steamClient.GetHandler<SteamFriends>();

            ClientConnection = new Callback<SteamClient.ConnectedCallback>(OnConnected, Globals.callbackManager);
            ClientLoggedOn = new Callback<SteamUser.LoggedOnCallback>(OnLoggedOn, Globals.callbackManager);
            ClientDisconnection = new Callback<SteamClient.DisconnectedCallback>(OnDisconnected, Globals.callbackManager);
            ClientLoggedOff = new Callback<SteamUser.LoggedOffCallback>(OnLoggedOff, Globals.callbackManager);
            ClientAccountInforationReceived = new Callback<SteamUser.AccountInfoCallback>(OnAccountInfo, Globals.callbackManager);

            JobUpdateAuthDetails = new JobCallback<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth, Globals.callbackManager);

            ClientChatMessage = new Callback<SteamFriends.ChatMsgCallback>(ChatRead.OnChatMessage, Globals.callbackManager);
            ClientFriendMessage = new Callback<SteamFriends.FriendMsgCallback>(ChatRead.OnPrivateMessage, Globals.callbackManager);
            ClientChatInvite = new Callback<SteamFriends.ChatInviteCallback>(ChatRead.OnChatInvite, Globals.callbackManager);
            ClientChatStatusChanged = new Callback<SteamFriends.ChatMemberInfoCallback>(ChatRead.OnChatMemberChange, Globals.callbackManager);
            ClientChatEntered = new Callback<SteamFriends.ChatEnterCallback>(OnJoinChat, Globals.callbackManager);

        }

        /// <summary>
        ///     Called when the first attempt to connect to steam has been made, and a response has been
        ///     received.
        ///     
        ///     
        /// </summary>
        /// <param name="callback"></param>
        private static void OnConnected(SteamClient.ConnectedCallback callback) {

            // We only care about a response that isn't EResult.OK here,
            // as we're only testing to see if we connected to the Steam servers
            if(callback.Result != EResult.OK) {

                ConsoleLog.Output(Globals.prefixes["general"], String.Format("Unable to connect to Steam: {0}", callback.Result), ConsoleColor.Magenta);

                return;
            }


            ConsoleLog.Output(Globals.prefixes["system"], String.Format("Connected to Steam! Logging in '{0}'...", user), ConsoleColor.Magenta);

            // We're connected, so lets see if we have a valid sentry file to validate ourselves with,
            // and read it into sentryHash.
            byte[] sentryHash = null;
            if(File.Exists("sentry.bin")) {
                // if we have a saved sentry file, read and sha-1 hash it
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }

            /* Attempt to login with our details,
             * username and password are whatever was given, AuthCode will either be null,
             * or set if this LogOn attempt fails.
             * The AuthCode is ignored if it is passed along with a sentry file,
             * though an error will be returned if the sentry is invalid.
             */
            Globals.steamUser.LogOn(new SteamUser.LogOnDetails {
                Username = user,
                Password = pass,

                // in this sample, we pass in an additional authcode
                // this value will be null (which is the default) for our first logon attempt
                AuthCode = authcode,

                // our subsequent logons use the hash of the sentry file as proof of ownership of the file
                // this will also be null for our first (no authcode) and second (authcode only) logon attempts
                SentryFileHash = sentryHash,
            });

        }

        /// <summary>
        /// 
        ///     Disconnected, either from entering an authcode, or the connection was lost.
        ///     
        ///     Attempts to reconnect with previous credentials every 5 seconds.
        ///     
        /// > set some sort of 'did log out' flag, to check if martec did logout
        /// </summary>
        /// <param name="obj"></param>
        private static void OnDisconnected(SteamClient.DisconnectedCallback obj) {

            ConsoleLog.Output(Globals.prefixes["system"], "Disconnected from Steam, reconnecting in 5...", ConsoleColor.Magenta);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            Globals.steamClient.Connect();

        }

        /// <summary>
        ///     Run after a LogOn attempt has been made
        /// </summary>
        /// <param name="callback"></param>
        private static void OnLoggedOn(SteamUser.LoggedOnCallback callback) {

            // Do something here to reprompt for password
            if(callback.Result == EResult.InvalidPassword) {

                ConsoleLog.Output(Globals.prefixes["general"], "Incorrect Password", ConsoleColor.Magenta);

                return;

            } else if(callback.Result == EResult.AccountLogonDenied) {
                ConsoleLog.Output(Globals.prefixes["system"], "This account is SteamGuard protected!", ConsoleColor.Magenta);
                ConsoleLog.Output(Globals.prefixes["system"], String.Format("Please enter the auth code sent to the email at {0}: ", callback.EmailDomain), ConsoleColor.Magenta);

                authcode = Console.ReadLine();
                return;
            }

            if(callback.Result != EResult.OK) {
                ConsoleLog.Output(Globals.prefixes["system"], String.Format("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult), ConsoleColor.Magenta);

                Program.programRunning = false;
                return;
            }

            ConsoleLog.Output(Globals.prefixes["system"], "Successfully logged on!", ConsoleColor.Magenta);
            //Program.programRunning = false;

            // at this point, we'd be able to perform actions on Steam
        }

        private static void OnLoggedOff(SteamUser.LoggedOffCallback obj) {

            Program.programRunning = false;

        }

        private static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback, JobID jobId) {

            ConsoleLog.Output(Globals.prefixes["system"], "Updating sentryfile...", ConsoleColor.Magenta);

            byte[] sentryHash = CryptoHelper.SHAHash(callback.Data);

            // write out our sentry file
            // ideally we'd want to write to the filename specified in the callback
            // but then this sample would require more code to find the correct sentry file to read during logon
            // for the sake of simplicity, we'll just use "sentry.bin"
            File.WriteAllBytes("sentry.bin", callback.Data);

            // inform the steam servers that we're accepting this sentry file
            Globals.steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails {
                JobID = jobId,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = callback.Data.Length,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            });

            ConsoleLog.Output(Globals.prefixes["system"], "Done!", ConsoleColor.Magenta);

        }
        private static void OnAccountInfo(SteamUser.AccountInfoCallback obj) {

            Globals.steamFriends.SetPersonaName(Globals.personaName);
            Globals.steamFriends.SetPersonaState(EPersonaState.Online);

            Globals.steamFriends.JoinChat(Globals.chatRoomID);

        }

        private static void OnJoinChat(SteamFriends.ChatEnterCallback obj) {

            ConsoleLog.Output(Globals.prefixes["system"], "Joined chat", ConsoleColor.Magenta);

            //Globals.steamFriends.KickChatMember(Globals.chatRoomID, 76561197994626023);


        }

    }
}
