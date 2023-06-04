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
    /// Administra los clientes del sistema
    /// </summary>
    public class ClientesController : Controller
    {
        private readonly TransportesContext _context;

        public ClientesController(TransportesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Pagina de inicio de clientes , obtiene por defecto la lista de clientes
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        /// <summary>
        /// filtra los clientes del sistemas con cualquier parametro del cliente
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IActionResult Search(string filter)
        {
            if (string.IsNullOrEmpty(filter)){
                return RedirectToAction(nameof(Index));
            }

            var clientes = _context.Clientes
                .Where(m => m.Cedula.ToString().Contains(filter) || 
                (!string.IsNullOrEmpty(m.Nombres) && m.Nombres.Contains(filter)) ||
                (!string.IsNullOrEmpty(m.Apellidos) && m.Apellidos.Contains(filter)) ||
                (m.Telefono.HasValue && m.Telefono.ToString().Contains(filter))
                );
            if (clientes == null || !clientes.Any())
            {
                return NotFound();
            }

            return View(nameof(Index), clientes);   
        }

        /// <summary>
        /// Abre la vista de creacion de clientes
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Metodo que crea un cliente segun lo recibido en el formulario
        /// </summary>
        /// <param name="cliente">cliente recibido</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Muestra la vista que edita el cliente
        /// </summary>
        /// <param name="cedula">cedula del cliente</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? cedula)
        {
            if (cedula == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(cedula);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        /// <summary>
        /// Metodo que edita un cliente segun lo recibido en el formulario
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cliente cliente)
        {
            if (!ClienteExists(cliente.Cedula))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Muestra la vista que elimina el cliente
        /// </summary>
        /// <param name="cedula">cedula del cliente</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(long? cedula)
        {
            if (cedula == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Cedula == cedula);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        /// <summary>
        /// Metodo que elimina un cliente
        /// </summary>
        /// <param name="cedula">cedula del cliente</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long cedula)
        {
            if (_context.Clientes == null)
            {
                return Problem("Contexto es nulo");
            }
            var cliente = await _context.Clientes.FindAsync(cedula);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// valida la existencia del cliente
        /// </summary>
        /// <param name="cedula"></param>
        /// <returns></returns>
        private bool ClienteExists(long cedula)
        {
          return (_context.Clientes?.Any(e => e.Cedula == cedula)).GetValueOrDefault();
        }
    }
}
