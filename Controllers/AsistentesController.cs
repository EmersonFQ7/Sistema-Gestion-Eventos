using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _repositorio;

    public AsistentesController(AsistenteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Asistentes
    public async Task<IActionResult> Index()
    {
        var asistentes = await _repositorio.ObtenerActivosAsync();

        return View(asistentes);
    }

    // GET: Asistentes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Asistentes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        // Validar que el DNI no esté registrado.
        if (!string.IsNullOrWhiteSpace(asistente.Dni) &&
            await _repositorio.ExisteDniAsync(asistente.Dni))
        {
            ModelState.AddModelError(
                nameof(asistente.Dni),
                "El DNI ingresado ya está registrado.");
        }

        // Validar que el correo no esté registrado.
        if (!string.IsNullOrWhiteSpace(asistente.Correo) &&
            await _repositorio.ExisteCorreoAsync(asistente.Correo))
        {
            ModelState.AddModelError(
                nameof(asistente.Correo),
                "El correo electrónico ingresado ya está registrado.");
        }

        if (!ModelState.IsValid)
        {
            return View(asistente);
        }

        asistente.Activo = true;

        await _repositorio.CrearAsync(asistente);

        TempData["Mensaje"] = "Asistente registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}