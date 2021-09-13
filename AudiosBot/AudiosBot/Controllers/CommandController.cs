using AudiosBot.Domain.Attributes;
using AudiosBot.Domain.Interfaces;
using AudiosBot.Domain.Models;
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
        public async Task<IActionResult> Busca()
        {
            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);

            var conteudo = await reader.ReadToEndAsync();

            var search = new Search(conteudo);

            Console.WriteLine($"CONTEUDO: {conteudo}");

            await _commandService.DefineAsync(search);

            return Ok();
        }

    }
}
