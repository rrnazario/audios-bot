using AudiosBot.Domain.Attributes;
using AudiosBot.Domain.Interfaces;
using AudiosBot.Domain.Models;
using AudiosBot.Infra.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AudiosBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly ICommandService _commandService;

        public CommandController(ICommandService commandService)
        {
            _commandService = commandService;
        }

        // GET api/command/search
        [HttpPost("Search")]
        [BasicAuth]
        public async Task<IActionResult> Search()
        {
            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);

            var content = await reader.ReadToEndAsync();

            var search = new Search(content);

            Console.WriteLine($"CONTENT: {content}");

            await _commandService.DefineAsync(search);

            LogHelper.Debug($"END '{nameof(Search)}'");

            return Ok();
        }

    }
}
