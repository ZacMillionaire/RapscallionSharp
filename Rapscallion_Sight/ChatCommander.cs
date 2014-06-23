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
    class ChatCommander {

        private static SteamFriends.ChatMsgCallback chatActor;
        private static SteamFriends.FriendMsgCallback privateActor;

        public ChatCommander() {

        }

        public static void TestForCommands(string chatMessage, SteamFriends.ChatMsgCallback callback) {

            chatActor = callback;

            PublicCommands(chatMessage);
            Roulette.TestForRoulette(chatMessage, callback);

        }


        public static void PrivateCommands(string chatMessage, SteamFriends.FriendMsgCallback callback) {

            privateActor = callback;

            if(callback.Sender != 76561197993698595) {
                return;
            }

            ChatCommander chatCommander = new ChatCommander();
            MethodInfo mi;
            GroupCollection paramList = null;

            Dictionary<string, string> rouletteCommands = new Dictionary<string, string>{
                {@"^!addAlias (.+) ([0-9]+)$","AddNewAlias"},
                {@"^!aliasList$","ListAliases"}
            };

            foreach(KeyValuePair<string, string> entry in rouletteCommands) {

                Regex rgx = new Regex(entry.Key);

                Match m = rgx.Match(chatMessage);

                if(m.Success) {

                    paramList = m.Groups;

                    mi = chatCommander.GetType().GetMethod(entry.Value);
                    mi.Invoke(chatCommander, new object[] { paramList });
                    return;

                }

            }

        } // End GenericCommandTest


        public static void PublicCommands(string chatMessage) {

            ChatCommander chatCommander = new ChatCommander();
            MethodInfo mi;
            GroupCollection paramList = null;

            Dictionary<string, string> rouletteCommands = new Dictionary<string, string>{
                {@"^!aliasList$","ListAliases"}
            };

            foreach(KeyValuePair<string, string> entry in rouletteCommands) {

                Regex rgx = new Regex(entry.Key);

                Match m = rgx.Match(chatMessage);

                if(m.Success) {

                    paramList = m.Groups;

                    mi = chatCommander.GetType().GetMethod(entry.Value);
                    mi.Invoke(chatCommander, new object[] { paramList });
                    return;

                }

            }

        } // End GenericCommandTest


        public static void AddNewAlias(GroupCollection aliasGroup) {

            string aliasName = aliasGroup[1].ToString();
            SteamID steamID = (SteamID) UInt64.Parse(aliasGroup[2].ToString());

            StreamWriter sw = File.AppendText("alias_list.txt");

            sw.WriteLine("{0}:{1}", aliasName, steamID.ConvertToUInt64());

            sw.Close();

            Roulette.BuildAliasList();

        } // End AddNewAlias


        public static void ListAliases(GroupCollection aliasGroup) {



            //Globals.steamFriends.SendChatRoomMessage(Globals.chatRoomID,EChatEntryType.ChatMsg,);

        }
    }
}
