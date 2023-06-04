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
    public class EnviosController : Controller
    {
        private readonly TransportesContext _context;

        public EnviosController(TransportesContext context)
        {
            _context = context;
        }

        // GET: Envios
        public async Task<IActionResult> Index()
        {
            var transportesContext = _context.Envios.Include(e => e.CedulaClienteNavigation).Include(e => e.DestinoNavigation).Include(e => e.TipoProductoNavigation).Include(e => e.VehiculoNavigation);
            return View(await transportesContext.ToListAsync());
        }

        // GET: Envios/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Envios == null)
            {
                return NotFound();
            }

            var envio = await _context.Envios
                .Include(e => e.CedulaClienteNavigation)
                .Include(e => e.DestinoNavigation)
                .Include(e => e.TipoProductoNavigation)
                .Include(e => e.VehiculoNavigation)
                .FirstOrDefaultAsync(m => m.NumeroGuia == id);
            if (envio == null)
            {
                return NotFound();
            }

            return View(envio);
        }

        // GET: Envios/Create
        public IActionResult Create()
        {
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "Cedula", "Cedula");
            ViewData["Destino"] = new SelectList(_context.Destinos, "IdDestino", "IdDestino");
            ViewData["TipoProducto"] = new SelectList(_context.TipoProductos, "IdTipoProducto", "IdTipoProducto");
            ViewData["Vehiculo"] = new SelectList(_context.Vehiculos, "Placa", "Placa");
            return View();
        }

        // POST: Envios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroGuia,FechaRegistro,FechaEntrega,Precio,CantidadProducto,CedulaCliente,TipoProducto,Vehiculo,Destino")] Envio envio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(envio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "Cedula", "Cedula", envio.CedulaCliente);
            ViewData["Destino"] = new SelectList(_context.Destinos, "IdDestino", "IdDestino", envio.Destino);
            ViewData["TipoProducto"] = new SelectList(_context.TipoProductos, "IdTipoProducto", "IdTipoProducto", envio.TipoProducto);
            ViewData["Vehiculo"] = new SelectList(_context.Vehiculos, "Placa", "Placa", envio.Vehiculo);
            return View(envio);
        }

        // GET: Envios/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Envios == null)
            {
                return NotFound();
            }

            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
            {
                return NotFound();
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "Cedula", "Cedula", envio.CedulaCliente);
            ViewData["Destino"] = new SelectList(_context.Destinos, "IdDestino", "IdDestino", envio.Destino);
            ViewData["TipoProducto"] = new SelectList(_context.TipoProductos, "IdTipoProducto", "IdTipoProducto", envio.TipoProducto);
            ViewData["Vehiculo"] = new SelectList(_context.Vehiculos, "Placa", "Placa", envio.Vehiculo);
            return View(envio);
        }

        // POST: Envios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NumeroGuia,FechaRegistro,FechaEntrega,Precio,CantidadProducto,CedulaCliente,TipoProducto,Vehiculo,Destino")] Envio envio)
        {
            if (id != envio.NumeroGuia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(envio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnvioExists(envio.NumeroGuia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "Cedula", "Cedula", envio.CedulaCliente);
            ViewData["Destino"] = new SelectList(_context.Destinos, "IdDestino", "IdDestino", envio.Destino);
            ViewData["TipoProducto"] = new SelectList(_context.TipoProductos, "IdTipoProducto", "IdTipoProducto", envio.TipoProducto);
            ViewData["Vehiculo"] = new SelectList(_context.Vehiculos, "Placa", "Placa", envio.Vehiculo);
            return View(envio);
        }

        // GET: Envios/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Envios == null)
            {
                return NotFound();
            }

            var envio = await _context.Envios
                .Include(e => e.CedulaClienteNavigation)
                .Include(e => e.DestinoNavigation)
                .Include(e => e.TipoProductoNavigation)
                .Include(e => e.VehiculoNavigation)
                .FirstOrDefaultAsync(m => m.NumeroGuia == id);
            if (envio == null)
            {
                return NotFound();
            }

            return View(envio);
        }

        // POST: Envios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Envios == null)
            {
                return Problem("Entity set 'TransportesContext.Envios'  is null.");
            }
            var envio = await _context.Envios.FindAsync(id);
            if (envio != null)
            {
                _context.Envios.Remove(envio);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnvioExists(string id)
        {
          return (_context.Envios?.Any(e => e.NumeroGuia == id)).GetValueOrDefault();
        }
    }
}
