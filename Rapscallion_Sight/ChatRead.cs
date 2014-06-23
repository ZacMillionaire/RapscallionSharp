using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rapscallion
{
    public class ChatRead
    {


        public static void OnChatMessage(SteamFriends.ChatMsgCallback callback) {

            string chatMessage = callback.Message.ToString();

            ulong chatUserAsID = callback.ChatterID.ConvertToUInt64();
            string chatUserAsName = Globals.steamFriends.GetFriendPersonaName(callback.ChatterID);

            ConsoleLog.Output(Globals.prefixes["log"],String.Format("{0}: {1}", chatUserAsName, chatMessage),ConsoleColor.DarkGreen);

            ChatCommander.TestForCommands(chatMessage, callback);

        }
        public static void OnPrivateMessage(SteamFriends.FriendMsgCallback callback) {

            string chatMessage = callback.Message.ToString();

            ChatCommander.PrivateCommands(chatMessage, callback);

        }
        public static void OnChatInvite(SteamFriends.ChatInviteCallback callback) {
            //throw new NotImplementedException();
        }
        public static void OnChatMemberChange(SteamFriends.ChatMemberInfoCallback callback) {

            // Rerwite this shit

            ulong actedByAsID = callback.StateChangeInfo.ChatterActedBy.ConvertToUInt64();
            ulong actedOnAsID = callback.StateChangeInfo.ChatterActedOn.ConvertToUInt64();

            int stateMethodInt = (int)callback.StateChangeInfo.StateChange;

            string stateMethod = callback.StateChangeInfo.StateChange.ToString().ToLower();
            string actedByAsName = Globals.steamFriends.GetFriendPersonaName(callback.StateChangeInfo.ChatterActedBy);
            string actedOnAsName = Globals.steamFriends.GetFriendPersonaName(callback.StateChangeInfo.ChatterActedOn);

            string message;
            string template;
            ConsoleColor backgroundColour;

            if(callback.StateChangeInfo.StateChange == EChatMemberStateChange.Kicked) {
                message = String.Format("{2} was {0} by {1}", stateMethod, actedByAsName, actedOnAsName); // kicked
                template = Globals.prefixes["kick"];
                backgroundColour = ConsoleColor.Yellow;
            } else if(callback.StateChangeInfo.StateChange == EChatMemberStateChange.Banned) {
                message = String.Format("{2} was {0} by {1}", stateMethod, actedByAsName, actedOnAsName); // banned
                template = Globals.prefixes["ban"];
                backgroundColour = ConsoleColor.Red;
            } else if(callback.StateChangeInfo.StateChange == EChatMemberStateChange.Disconnected) {
                message = String.Format("{1} {0} from chat.", stateMethod, actedByAsName);
                template = Globals.prefixes["general"];
                backgroundColour = ConsoleColor.DarkGreen;
            } else {
                message = String.Format("{1} {0} chat.", stateMethod, actedByAsName);
                template = Globals.prefixes["general"];
                backgroundColour = ConsoleColor.DarkGreen;
            }

            ConsoleLog.Output(template, message, backgroundColour);

        }

    }
}
