
using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio
{
    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Inscripcion>> ObtenerTodasAsync()
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    public async Task<List<Evento>> ObtenerEventosActivosAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<List<Asistente>> ObtenerAsistentesActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerEventoAsync(int eventoId)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e =>
                e.Id == eventoId && e.Activo);
    }

    public async Task<bool> ExisteInscripcionConfirmadaAsync(
        int eventoId, int asistenteId)
    {
        return await _context.Inscripciones
            .AnyAsync(i =>
                i.EventoId == eventoId &&
                i.AsistenteId == asistenteId &&
                i.Estado == "Confirmada");
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId)
    {
        return await _context.Inscripciones
            .CountAsync(i =>
                i.EventoId == eventoId &&
                i.Estado == "Confirmada");
    }

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.Id == id);

        if (inscripcion == null ||
            inscripcion.Estado != "Confirmada")
        {
            return false;
        }

        inscripcion.Estado = "Cancelada";

        await _context.SaveChangesAsync();
        return true;
    }
}
