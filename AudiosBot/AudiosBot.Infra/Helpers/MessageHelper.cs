using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiosBot.Infra.Helpers
{
    public class MessageHelper
    {
        public static string GetRandomSearchingMessage() => "SearchingMessages[random.Next(0, SearchingMessages.Length)]";
    }
}
