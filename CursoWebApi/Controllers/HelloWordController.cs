using CursoWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CursoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class HelloWordController : ControllerBase
    {
        private readonly ILogger<HelloWordController> _logger;
        IHelloWordService helloWordService;

        TareasContext dbcontext;

        public HelloWordController(IHelloWordService helloWord,ILogger<HelloWordController> logger, TareasContext db)
        {
            _logger = logger;
            helloWordService = helloWord;
            dbcontext = db;
        }


        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            _logger.LogInformation("Retornando Hello Word ");
            return Ok(helloWordService.GetHelloWord());
        }

        [HttpGet]
        [Route("createdb")]
        public IActionResult CreateDatabase()
        {
            dbcontext.Database.EnsureCreated();
            return Ok();
        }
    }

}