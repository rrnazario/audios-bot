using Newtonsoft.Json;
using System;
using Telegram.Bot.Types;

namespace AudiosBot.Domain.Models
{
    public class Search
    {
        private string _termo;
        public string Term
        {
            get => _termo.Replace("partitura", "", StringComparison.InvariantCultureIgnoreCase);
            set => _termo = value;
        }
        public User User { get; set; }

        public Search(Message message)
        {
            Term = message.Text;
            User = new(message.Chat.FirstName,
                       message.Chat.LastName,
                       message.Chat.Id.ToString(),
                       message.Chat.Id.ToString());
        }

        public Search(string conteudo) : this(JsonConvert.DeserializeObject(conteudo)) { }

        public Search(dynamic conteudoDinamico)
        {
            if (conteudoDinamico.ToString().Contains("queryResult"))
                ParseDialogflow(conteudoDinamico);
            else
                ParseDebug(conteudoDinamico);

            Console.WriteLine($"CONSULTA: {Term}");
            Console.WriteLine($"NOME BUSCADOR: {User.Name}");
        }

        public void ParseDialogflow(dynamic conteudoDinamico)
        {
            Console.WriteLine($"RELEASE MODE");

            Term = (string)conteudoDinamico["queryResult"]["queryText"];

            User = new User(conteudoDinamico);
        }

        public void ParseDebug(dynamic conteudoDinamico)
        {
            Console.WriteLine($"DEBUG MODE");

            Term = (string)conteudoDinamico["consulta"];

            try
            {
                var nome = (string)conteudoDinamico["nomeBuscador"];
                var chatId = (string)conteudoDinamico["chatId"];

                User = new User(nome, "", chatId, chatId);
            }
            catch { }
        }
    }
}
