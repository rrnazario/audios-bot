using AudiosBot.Infra.Constants;
using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AudiosBot.Infra.Extensions
{
    public static class TelegramExtensions
    {
        public static string GetFileId(this Message message)
        {
            return message.Type switch
            {
                MessageType.Audio => message.Audio.FileId,
                MessageType.Document => message.Document.FileId,
                MessageType.Voice => message.Voice.FileId,
                _ => ""
            };
        }

        public static string GetFileName(this Message message)
        {
            return message.Type switch
            {
                MessageType.Audio => message.Audio.FileName,
                MessageType.Document => message.Document.FileName,
                MessageType.Voice => $"{message.Voice.FileId}.{message.Voice.MimeType.Split("/").Last()}",
                _ => ""
            };
        }

        public static bool IsValidAudioFile(this Message message)
        {
            var allowedMessageTypes = new MessageType[] { MessageType.Audio, MessageType.Document, MessageType.Voice };

            return allowedMessageTypes.Contains(message.Type);
        }

        public static bool IsUserAdmin(this Message message) 
            => AdminConstants.AdminChatId.Split(",").Contains(message.Chat.Id.ToString());            
    }
}
