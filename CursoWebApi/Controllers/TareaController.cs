using CursoWebApi.Models;
using CursoWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CursoWebApi.Controllers
{
    [Route ("api/[controller]")]
    public class TareaController : ControllerBase
    {
        ITareasService tareasService;
        public TareaController (ITareasService service)
        {
            tareasService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok (tareasService.Get());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Tarea tarea)
        {
            tareasService.Save(tarea);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Put(Guid id, [FromBody] Tarea tarea)
        {
            tareasService.Update(id, tarea);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            tareasService.Delete(id);
            return Ok();
        }
    }
}
