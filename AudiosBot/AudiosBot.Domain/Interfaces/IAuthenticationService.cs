using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiosBot.Domain.Interfaces
{
    public interface IAuthenticationService
    {
        bool IsValidUser(string username, string password);
    }
}
