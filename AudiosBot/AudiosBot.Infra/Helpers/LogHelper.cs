using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiosBot.Infra.Helpers
{
    public class LogHelper
    {
        public static void Debug(string message) => Console.WriteLine(message);
        public static void Error(Exception e) => Console.WriteLine($"{e.Message}\n\n{e.StackTrace}");
    }
}
