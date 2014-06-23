using SteamKit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rapscallion {
    public class Roulette {

        private static Random rand = new Random();
        private static SteamFriends.ChatMsgCallback chatActor;
        private static Dictionary<string, SteamID> aliasList = new Dictionary<string, SteamID>();

        public Roulette() {
        }


        public Dictionary<string, SteamID> AliasList{
            get {
                return aliasList;
            }
        }


        public static void BuildAliasList() {

            aliasList = new Dictionary<string, SteamID>();

            string[] split;
            string[] lines = File.ReadAllLines("alias_list.txt");

            string aliasName;
            SteamID steamID;

            foreach(string line in lines) {

                if(line.Equals("")) {
                    break;
                }
                split = line.Split(':');

                aliasName = split[0];
                steamID = UInt64.Parse(split[1]);

                aliasList.Add(aliasName, steamID);
            }

        }


        public static void TestForRoulette(string chatMessage, SteamFriends.ChatMsgCallback callback) {

            Roulette roulette = new Roulette();
            MethodInfo mi;

            chatActor = callback;

            Dictionary<string, string> rouletteCommands = new Dictionary<string, string>{
                {@"^!roulette\s*(.+)?$","RegularRoulette"}
            };

            foreach(KeyValuePair<string,string> entry in rouletteCommands){

                Regex rgx = new Regex(entry.Key);

                Match m = rgx.Match(chatMessage);

                if(m.Success) {

                    GroupCollection g = m.Groups;

                    mi = roulette.GetType().GetMethod(entry.Value);
                    mi.Invoke(roulette, new string[]{g[1].ToString()});
                    return;
                }
            }

        }


        private static SteamID TryFindSteamIDFromAlias(string alias) {

            SteamID aliasSteamID;
            bool tryGetSteamID;

            tryGetSteamID = aliasList.TryGetValue(alias, out aliasSteamID);

            if(!tryGetSteamID) {
                aliasSteamID = chatActor.ChatterID;
            }

            return aliasSteamID;

        }


        public static void RegularRoulette(string alias) {

            SteamID userToKick = chatActor.ChatterID;

            if(alias.Equals("")) {
                Console.WriteLine("self roulette");
            } else {
                userToKick = TryFindSteamIDFromAlias(alias);
                Console.WriteLine("alias roulette for {0}",alias);
            }

            int randResult;

            int low = 0;
            int high = 100;

            randResult = rand.Next(low,high+1);

            ConsoleLog.Output(
                Globals.prefixes["kick"],
                String.Format(
                    "{0} spun the wheel of !roulette and landed on {1}",
                    Globals.steamFriends.GetFriendPersonaName(userToKick),
                    randResult
                ),
                ConsoleColor.Yellow
            );

            if(randResult >= 80 && randResult < 95) {

                Globals.steamFriends.KickChatMember(Globals.chatRoomID, userToKick);

                ConsoleLog.Output(
                    Globals.prefixes["kick"],
                    String.Format(
                        "{0} was kicked by !roulette",
                        Globals.steamFriends.GetFriendPersonaName(userToKick)
                    ),
                    ConsoleColor.Yellow
                );

                Globals.steamFriends.SendChatRoomMessage(
                    Globals.chatRoomID,
                    EChatEntryType.ChatMsg,
                    "Winner~"
                );

            } else if(randResult >= 95) {

                Globals.steamFriends.KickChatMember(Globals.chatRoomID, userToKick);

                ConsoleLog.Output(
                    Globals.prefixes["kick"],
                    String.Format(
                        "{0} was banned by !roulette",
                        Globals.steamFriends.GetFriendPersonaName(userToKick)
                    ),
                    ConsoleColor.Yellow
                );

                Globals.steamFriends.SendChatRoomMessage(
                    Globals.chatRoomID,
                    EChatEntryType.ChatMsg,
                    "Get rekt"
                );

            } else {

                Globals.steamFriends.SendChatRoomMessage(
                    Globals.chatRoomID,
                    EChatEntryType.ChatMsg,
                    "Better(?) luck next time"
                );

            }

        } // End RegularRoulette


    } // End Class


} // End namespace
 