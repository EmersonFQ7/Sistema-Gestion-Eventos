
using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Evento>> ObtenerActivosAsync(
        int? categoriaId = null)
    {
        var consulta = _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo);

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(
                e => e.CategoriaId == categoriaId.Value);
        }

        return await consulta
            .Include(e => e.Categoria)
            .Include(e => e.Inscripciones.Where(
                i => i.Estado == "Confirmada"))
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Categoria>> ObtenerCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task CrearAsync(Evento evento)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Evento evento)
    {
        var existente = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == evento.Id);

        if (existente == null || !existente.Activo)
            return false;

        existente.Titulo = evento.Titulo;
        existente.Fecha = evento.Fecha;
        existente.Lugar = evento.Lugar;
        existente.Aforo = evento.Aforo;
        existente.Precio = evento.Precio;
        existente.CategoriaId = evento.CategoriaId;
        existente.Modalidad = evento.Modalidad;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DesactivarAsync(int id)
    {
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento == null || !evento.Activo)
            return false;

        evento.Activo = false;

        await _context.SaveChangesAsync();
        return true;
    }
}
