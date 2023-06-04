using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transportes.Models;

namespace Transportes.Controllers
{
    /// <summary>
    /// Administra los tipos de productos del sistema
    /// </summary>
    public class TipoProductosController : Controller
    {
        private readonly TransportesContext _context;

        public TipoProductosController(TransportesContext context)
        {
            _context = context;
        }

       /// <summary>
       /// Carga todos los tipos de productos del sistema
       /// </summary>
       /// <returns></returns>
        public async Task<IActionResult> Index()
        {
              return _context.TipoProductos != null ? 
                          View(await _context.TipoProductos.ToListAsync()) :
                          Problem("Contexto es nulo");
        }

        /// <summary>
        /// Filtra los productos del sistema
        /// </summary>
        /// <param name="descripcion"></param>
        /// <returns></returns>
        public async Task<IActionResult> Search(string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion))
            {
                return RedirectToAction(nameof(Index));
            }

            var tipoProducto = _context.TipoProductos
                .Where(m => m.Descripcion.Contains(descripcion));

            var tipoProductoList = await tipoProducto.ToListAsync();

            if (tipoProductoList == null || !tipoProductoList.Any())
            {
                return NotFound();
            }

            return View(nameof(Index), tipoProductoList);
        }

        /// <summary>
        /// Muestra la vista que crea tipos de productos
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Permite crear productos
        /// </summary>
        /// <param name="tipoProducto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoProducto tipoProducto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoProducto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Muestra la vista que permite editar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TipoProductos == null)
            {
                return NotFound();
            }

            var tipoProducto = await _context.TipoProductos.FindAsync(id);
            if (tipoProducto == null)
            {
                return NotFound();
            }
            return View(tipoProducto);
        }

        /// <summary>
        /// Edita un tipo de producto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoProducto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoProducto tipoProducto)
        {
            if (!TipoProductoExists(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(tipoProducto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Muestra la vista que permite eliminar un registro
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TipoProductos == null)
            {
                return NotFound();
            }

            var tipoProducto = await _context.TipoProductos
                .FirstOrDefaultAsync(m => m.IdTipoProducto == id);
            if (tipoProducto == null)
            {
                return NotFound();
            }

            return View(tipoProducto);
        }

        /// <summary>
        /// Elimina un producto del sistema
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TipoProductos == null)
            {
                return Problem("Contexto es nulo");
            }
            var tipoProducto = await _context.TipoProductos.FindAsync(id);
            if (tipoProducto != null)
            {
                _context.TipoProductos.Remove(tipoProducto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Valida la existencia de un producto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool TipoProductoExists(int id)
        {
          return (_context.TipoProductos?.Any(e => e.IdTipoProducto == id)).GetValueOrDefault();
        }
    }
}
