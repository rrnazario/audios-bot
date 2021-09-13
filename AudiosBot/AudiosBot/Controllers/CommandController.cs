using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudiosBot.API.Controllers
{
    public class CommandController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
