using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiosBot.Infra.Constants
{
    public class Enums
    {
        public enum SendFileResult
        {
            SUCCESS,
            BLOCKED_BY_USER,
            GENERAL_ERROR
        }
    }
}
