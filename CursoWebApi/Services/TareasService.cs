using CursoWebApi.Models;

namespace CursoWebApi.Services
{
    public class TareasService : ITareasService
    {
        TareasContext context;

        public TareasService(TareasContext dbcontext)
        {
            context = dbcontext;
        }
        public IEnumerable<Tarea> Get()
        {
            return context.Tareas;
        }

        public async Task Save(Tarea tareas)
        {
            context.Add(tareas);
            await context.SaveChangesAsync();
        }

        public async Task Update(Guid id, Tarea tareas)
        {
            var tareaActual = context.Tareas.Find(id);
            if (tareaActual != null)
            {
                tareaActual.Titulo = tareas.Titulo;
                tareaActual.Descripcion = tareas.Descripcion;
                tareaActual.PrioridadTarea = tareas.PrioridadTarea;
                tareaActual.FechaCreacion = tareas.FechaCreacion;
                tareaActual.Categoria = tareas.Categoria;
                tareaActual.Resumen = tareas.Resumen;

                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid id)
        {
            var tareaActual = context.Tareas.Find(id);
            if (tareaActual != null)
            {
                context.Remove(tareaActual);
                await context.SaveChangesAsync();
            }
        }


    }

    public interface ITareasService
    {
        IEnumerable<Tarea> Get();
        Task Save(Tarea tareas);
        Task Update(Guid id, Tarea tareas);
        Task Delete(Guid id);
    }
}
