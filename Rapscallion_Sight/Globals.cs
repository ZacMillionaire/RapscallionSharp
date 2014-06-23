using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace Rapscallion {

    class Globals {

        public static SteamClient steamClient = new SteamClient();
        public static CallbackManager callbackManager = new CallbackManager(steamClient);

        public static SteamUser steamUser;
        public static SteamFriends steamFriends;

        public static Dictionary<string,string> prefixes = new Dictionary<string,string>(){
            {"general","[GENERAL]"},
            {"log","[LOGGED]"},
            {"message","[MSG]"},
            {"link","[LINK]"},
            {"kick","[KICK]"},
            {"ban","[BAN]"},
            {"system","[SYS]"}
        };

        public const long chatRoomID = 103582791429601458;
        public static string personaName = "Martec r3vival";

    }
}
