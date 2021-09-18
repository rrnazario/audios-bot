using AudiosBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiosBot.Domain.Interfaces
{
    public interface ICommandService
    {
        Task DefineAsync(Search search);
        Task SearchAsync(Search search);
    }
}
