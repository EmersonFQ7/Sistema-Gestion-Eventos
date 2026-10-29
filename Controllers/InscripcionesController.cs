
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _repositorio;

    public InscripcionesController(InscripcionRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IActionResult> Index()
    {
        var inscripciones = await _repositorio.ObtenerTodasAsync();
        return View(inscripciones);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        var evento = await _repositorio.ObtenerEventoAsync(
            inscripcion.EventoId);

        var asistentes = await _repositorio.ObtenerAsistentesActivosAsync();

        if (evento == null)
        {
            ModelState.AddModelError(
                nameof(inscripcion.EventoId),
                "Selecciona un evento activo válido.");
        }

        if (!asistentes.Any(a => a.Id == inscripcion.AsistenteId))
        {
            ModelState.AddModelError(
                nameof(inscripcion.AsistenteId),
                "Selecciona un asistente activo válido.");
        }

        if (evento != null)
        {
            if (evento.Fecha.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(inscripcion.EventoId),
                    "No se puede inscribir en un evento cuya fecha ya pasó.");
            }

            if (await _repositorio.ExisteInscripcionConfirmadaAsync(
                inscripcion.EventoId, inscripcion.AsistenteId))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El asistente ya tiene una inscripción confirmada para este evento.");
            }

            int confirmadas = await _repositorio.ContarConfirmadasAsync(
                inscripcion.EventoId);

            if (confirmadas >= evento.Aforo)
            {
                ModelState.AddModelError(
                    nameof(inscripcion.EventoId),
                    "El evento ya alcanzó su aforo máximo.");
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(
                inscripcion.EventoId,
                inscripcion.AsistenteId);

            return View(inscripcion);
        }

        inscripcion.FechaInscripcion = DateTime.Now;
        inscripcion.Estado = "Confirmada";

        await _repositorio.CrearAsync(inscripcion);

        TempData["Mensaje"] = "Inscripción registrada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        bool cancelada = await _repositorio.CancelarAsync(id);

        if (!cancelada)
        {
            return NotFound();
        }

        TempData["Mensaje"] = "Inscripción cancelada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(
        int? eventoId = null,
        int? asistenteId = null)
    {
        var eventos = await _repositorio.ObtenerEventosActivosAsync();
        var asistentes = await _repositorio.ObtenerAsistentesActivosAsync();

        ViewBag.EventoId = new SelectList(
            eventos, "Id", "Titulo", eventoId);

        ViewBag.AsistenteId = new SelectList(
            asistentes, "Id", "Nombres", asistenteId);
    }
}
