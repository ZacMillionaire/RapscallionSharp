using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapscallion {
    class ConsoleLog {

        public ConsoleLog() {

        }
        public static void Output(string Message) {

        }
        public static void Output(string messagePrefix, string Message, ConsoleColor foregroundColour) {
            Console.ForegroundColor = foregroundColour;
            Console.WriteLine(messagePrefix + " " + Message);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
