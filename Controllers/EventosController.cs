using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _repositorio;

    public EventosController(EventoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Eventos
    public async Task<IActionResult> Index(int? categoriaId)
    {
        var eventos = await _repositorio.ObtenerActivosAsync(categoriaId);
        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();

        ViewBag.Categorias = new SelectList(
            categorias, "Id", "Nombre", categoriaId);

        ViewBag.CategoriaId = categoriaId;

        return View(eventos);
    }

    // GET: Eventos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var evento = await _repositorio.ObtenerPorIdAsync(id.Value);

        if (evento == null || !evento.Activo)
            return NotFound();

        return View(evento);
    }

    // GET: Eventos/Create
    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        return View();
    }

    // POST: Eventos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();

        if (!categorias.Any(c => c.Id == evento.CategoriaId))
        {
            ModelState.AddModelError(
                nameof(evento.CategoriaId),
                "Selecciona una categoría activa válida.");

            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        await _repositorio.CrearAsync(evento);

        TempData["Mensaje"] = "Evento registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Eventos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var evento = await _repositorio.ObtenerPorIdAsync(id.Value);

        if (evento == null || !evento.Activo)
            return NotFound();

        await CargarCategoriasAsync(evento.CategoriaId);

        return View(evento);
    }

    // POST: Eventos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();

        if (!categorias.Any(c => c.Id == evento.CategoriaId))
        {
            ModelState.AddModelError(
                nameof(evento.CategoriaId),
                "Selecciona una categoría activa válida.");

            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        var actualizado = await _repositorio.ActualizarAsync(evento);

        if (!actualizado)
            return NotFound();

        TempData["Mensaje"] = "Evento actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Eventos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var evento = await _repositorio.ObtenerPorIdAsync(id.Value);

        if (evento == null || !evento.Activo)
            return NotFound();

        return View(evento);
    }

    // POST: Eventos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var desactivado = await _repositorio.DesactivarAsync(id);

        if (!desactivado)
            return NotFound();

        TempData["Mensaje"] = "Evento desactivado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategoriasAsync(int? categoriaId = null)
    {
        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();

        ViewBag.CategoriaId = new SelectList(
            categorias, "Id", "Nombre", categoriaId);
    }
}