using System;
using System.Collections.Generic;
using System.Linq;

namespace AudiosBot.Infra.Constants
{
    public class AdminConstants
    {
        public static readonly string TelegramBotToken = Environment.GetEnvironmentVariable("TelegramBotToken");

        public static readonly string AdminChatId = Environment.GetEnvironmentVariable("AdminChatId");
        public static readonly string AdminMail = Environment.GetEnvironmentVariable("AdminMail");
        public static readonly Dictionary<string, string> Users = Environment.GetEnvironmentVariable("Users")
                                                                  .Split(",")
                                                                  .ToDictionary(k => k.Split("|").First(), v => v.Split("|").Last());
    }
}
