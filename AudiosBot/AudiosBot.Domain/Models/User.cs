using AudiosBot.Infra.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiosBot.Domain.Models
{
    public class User
    {
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _chatId;
        private readonly bool _debug;
        public int SheetIndex;
        public string Id { get; set; }
        public string Name => !string.IsNullOrEmpty(_firstName) ? $"{_firstName} {_lastName}" : "Humano";
        public bool Debug => _debug;

        public bool IsAdmin => AdminConstants.AdminChatId.Split(",").Contains(Id);
        public User(string firstName, string lastName, string id, string chatId, bool debug = false)
        {
            _firstName = firstName;
            _lastName = lastName;
            Id = id;
            _chatId = chatId;
            _debug = debug;
        }

        public User(dynamic conteudoDinamico)
        {
            try
            {
                _firstName = (string)conteudoDinamico["originalDetectIntentRequest"]["payload"]["data"]["from"]["first_name"];
                _lastName = (string)conteudoDinamico["originalDetectIntentRequest"]["payload"]["data"]["from"]["last_name"];
                Id = (string)conteudoDinamico["originalDetectIntentRequest"]["payload"]["data"]["from"]["id"];
                _chatId = (string)conteudoDinamico["originalDetectIntentRequest"]["payload"]["data"]["chat"]["id"];
            }
            catch (Exception ex)
            {
                //LogHelper.Error(ex);
            }
        }

        public User(string linha) //Leonardo|De Freitas|987843905|987843905
        {
            var array = linha.Split('|');

            _firstName = array[0];
            _lastName = array[1];
            Id = _chatId = array[2];
        }

        public override string ToString() => string.Join("|", _firstName, _lastName, Id, _chatId);
    }
}
