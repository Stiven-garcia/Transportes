using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transportes.Models;

namespace Transportes.Controllers
{
    /// <summary>
    /// Administra los destinos (puertos y bodegas) de los envios
    /// </summary>
    [AllowAnonymous]
    public class DestinosController : Controller
    {
        private readonly TransportesContext _context;

        public DestinosController(TransportesContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Pagina de inicio de Destinos , obtiene por defecto la lista de destinos
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            ViewData["TipoDestino"] = new SelectList(_context.TipoDestinos, "IdTipoDestino", "Tipo");
            var destinos = _context.Destinos.Include(d => d.TipoDestinoNavigation);
            return View(await destinos.ToListAsync());
        }

        /// <summary>
        /// filtra destino
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Search(int? tipoDestino, string nombreDestino, string ubicacionDestino)
        {
            var destino = _context.Destinos.Where(x => (tipoDestino == null || x.TipoDestino == tipoDestino)
            && (string.IsNullOrEmpty(nombreDestino) || x.Nombre.Contains(nombreDestino))
            && (string.IsNullOrEmpty(ubicacionDestino) || x.Ubicacion.Contains(ubicacionDestino))
            );
            var destinosList = await destino.ToListAsync();
            if (destinosList == null || !destinosList.Any())
            {
                return NotFound();
            }
            ViewData["TipoDestino"] = new SelectList(_context.TipoDestinos, "IdTipoDestino", "Tipo");
            return View(nameof(Index), destinosList);
        }

        /// <summary>
        /// redirecciona a la vista crear destinos
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewData["TipoDestino"] = new SelectList(_context.TipoDestinos, "IdTipoDestino", "Tipo");
            return View();
        }

        /// <summary>
        /// crea un destino con la informacion obtenida de la vista
        /// </summary>
        /// <param name="destino"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Destino destino)
        {
            if (ModelState.IsValid)
            {
                _context.Add(destino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Muestra la vista que permite editar un registro
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Destinos == null)
            {
                return NotFound();
            }

            var destino = await _context.Destinos.Include(d => d.TipoDestinoNavigation).FirstOrDefaultAsync(x => x.IdDestino == id);
            if (destino == null)
            {
                return NotFound();
            }
            ViewData["TipoDestino"] = new SelectList(_context.TipoDestinos, "IdTipoDestino", "Tipo");
            return View(destino);
        }

        /// <summary>
        /// Permite editar un registro con la informacion de la vista
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destino"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Destino destino)
        {
            if (!DestinoExists(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(destino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Muestra la vista que permite eliminar un destino
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Destinos == null)
            {
                return NotFound();
            }

            var destino = await _context.Destinos
                .Include(d => d.TipoDestinoNavigation)
                .FirstOrDefaultAsync(m => m.IdDestino == id);
            if (destino == null)
            {
                return NotFound();
            }

            return View(destino);
        }

       /// <summary>
       /// Elimina un registro
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Destinos == null)
            {
                return Problem("Contexto es nulo");
            }
            var destino = await _context.Destinos.FindAsync(id);
            if (destino != null)
            {
                _context.Destinos.Remove(destino);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// valida si un destino existe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool DestinoExists(int id)
        {
          return (_context.Destinos?.Any(e => e.IdDestino == id)).GetValueOrDefault();
        }
    }
}
