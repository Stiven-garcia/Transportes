using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transportes.Models;

namespace Transportes.Controllers
{
    /// <summary>
    /// Administras los envios del sistema 1 == terrestre, 2 == maritimo
    /// </summary>
    [AllowAnonymous]
    public class EnviosController : Controller
    {
        private readonly TransportesContext _context;
        private readonly int Maritimo = 2;
        private readonly int Terrestre = 1;

        public EnviosController(TransportesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista los envios del sistema
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            ViewData["TipoEnvio"] = new SelectList(new Dictionary<int, string>()
            {
                {2,"Terrestre"}, // configuracion base de datos
                {1,"Maritimo"},
            }, "Key", "Value");
            ViewData["TipoVehiculo"] = new SelectList(_context.TipoVehiculos, "IdTipoVehiculo", "Tipo");
            ViewData["TipoDestino"] = new SelectList(_context.TipoDestinos, "IdTipoDestino", "Tipo");
            
            var envios = _context.Envios.Include(e => e.CedulaClienteNavigation).Include(e => e.DestinoNavigation).Include(e => e.TipoProductoNavigation).Include(e => e.VehiculoNavigation);
            return View(await envios.ToListAsync());
        }

        /// <summary>
        /// Filtra Envios
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(EnviosSearchRequest filter)
        {
            var envios = _context.Envios.Include(x => x.VehiculoNavigation).Include(x => x.CedulaClienteNavigation).
                Include(x => x.TipoProductoNavigation).Include(x => x.DestinoNavigation).
                Where(x => (string.IsNullOrEmpty(filter.NumeroGuia) || x.NumeroGuia.Contains(filter.NumeroGuia))
            && (!filter.TipoEnvio.HasValue || x.VehiculoNavigation.TipoVehiculo == filter.TipoEnvio)
             && (!filter.FechaRegistroStart.HasValue || x.FechaRegistro >= filter.FechaRegistroStart)
             && (!filter.FechaRegistroEnd.HasValue || x.FechaRegistro <= filter.FechaRegistroEnd)
             && (!filter.FechaEntregaStart.HasValue || x.FechaEntrega >= filter.FechaEntregaStart)
             && (!filter.FechaEntregaEnd.HasValue || x.FechaEntrega <= filter.FechaEntregaEnd)
             && (!filter.PrecioStart.HasValue || x.Precio >= filter.PrecioStart)
             && (!filter.PrecioEnd.HasValue || x.Precio <= filter.PrecioEnd)
             && (!filter.Cedula.HasValue || x.CedulaCliente.ToString().Contains(filter.Cedula.ToString()))
              && (string.IsNullOrEmpty(filter.NombreApellido) || x.CedulaClienteNavigation.Nombres.Contains(filter.NombreApellido.ToString())
                    || x.CedulaClienteNavigation.Apellidos.Contains(filter.NombreApellido.ToString()))
              && (string.IsNullOrEmpty(filter.TipoProducto) || x.TipoProductoNavigation.Descripcion.Contains(filter.TipoProducto.ToString()))
              && (string.IsNullOrEmpty(filter.Placa) || x.Vehiculo.Contains(filter.Placa.ToString()))
              && (!filter.TipoVehiculo.HasValue || x.VehiculoNavigation.TipoVehiculo == filter.TipoVehiculo)
              && (string.IsNullOrEmpty(filter.Destino) || x.DestinoNavigation.Nombre.Contains(filter.Destino.ToString()))
              && (!filter.TipoDestino.HasValue || x.DestinoNavigation.TipoDestino == filter.TipoDestino)
            );

            var enviosList = await envios.ToListAsync();
            if (enviosList == null || !enviosList.Any())
            {
                return NotFound();
            }

            ViewData["TipoEnvio"] = new SelectList(new Dictionary<int, string>()
            {
                {2,"Terrestre"}, // configuracion base de datos
                {1,"Maritimo"},
            },"Key","Value");
            ViewData["TipoVehiculo"] = new SelectList(_context.TipoVehiculos, "IdTipoVehiculo", "Tipo");
            ViewData["TipoDestino"] = new SelectList(_context.TipoDestinos, "IdTipoDestino", "Tipo");

            return View(nameof(Index), enviosList);
        }

        /// <summary>
        /// Muestra a detalle un envio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id, int tipo)
        {
            if(tipo != Maritimo && tipo != Terrestre)
            {
                return BadRequest("El tipo de envio es incorrecto");
            }

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
            ViewData["TipoEnvio"] = tipo == Maritimo ? "Maritimo" : "Terrestre";
            return View(envio);
        }

        /// <summary>
        /// Muestra la vista que permite crear un envio
        /// </summary>
        /// <returns></returns>
        public IActionResult Create(int tipo)
        {

            if (tipo != Maritimo && tipo != Terrestre)
            {
                return BadRequest("El tipo de envio es incorrecto");
            }

            var tipoDestino = tipo == Maritimo ? "Puerto" : "Bodega";
            var tipoVehiculo= tipo == Maritimo ? "Flota" : "Camion";
            ViewData["TipoEnvio"] = tipo == Maritimo ? "Maritimo" : "Terrestre";

            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "Cedula", "Cedula");
            ViewData["Destino"] = new SelectList(_context.Destinos.Include(x => x.TipoDestinoNavigation).Where(x => x.TipoDestinoNavigation.Tipo.Equals(tipoDestino)), "IdDestino", "Nombre");
            ViewData["TipoProducto"] = new SelectList(_context.TipoProductos, "IdTipoProducto", "Descripcion");
            ViewData["Vehiculo"] = new SelectList(_context.Vehiculos.Include(x=> x.TipoVehiculoNavigation).Where(x =>x.TipoVehiculoNavigation.Tipo.Equals(tipoVehiculo)), "Placa", "Placa");
            return View();
        }

        /// <summary>
        /// Permite crear un envio
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Envio envio)
        {
            if (!string.IsNullOrEmpty(envio.NumeroGuia) && EnvioExists(envio.NumeroGuia))
            {
                return UnprocessableEntity("Ya existe el numero de guia");
            }

            if (envio.FechaEntrega <= envio.FechaRegistro)
            {
                return UnprocessableEntity("La fecha de entrega debe ser mayor a la del registro");
            }

            if (ModelState.IsValid)
            {
                _context.Add(envio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Muestra la vista que permiute editar un envio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id, int tipo)
        {
            if (id == null || _context.Envios == null)
            {
                return NotFound();
            }
            if (tipo != Maritimo && tipo != Terrestre)
            {
                return BadRequest("El tipo de envio es incorrecto");
            }

            var tipoDestino = tipo == Maritimo ? "Puerto" : "Bodega";
            var tipoVehiculo = tipo == Maritimo ? "Flota" : "Camion";
            ViewData["TipoEnvio"] = tipo == Maritimo ? "Maritimo" : "Terrestre";

            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
            {
                return NotFound();
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "Cedula", "Cedula");
            ViewData["Destino"] = new SelectList(_context.Destinos.Include(x => x.TipoDestinoNavigation).Where(x => x.TipoDestinoNavigation.Tipo.Equals(tipoDestino)), "IdDestino", "Nombre");
            ViewData["TipoProducto"] = new SelectList(_context.TipoProductos, "IdTipoProducto", "Descripcion");
            ViewData["Vehiculo"] = new SelectList(_context.Vehiculos.Include(x => x.TipoVehiculoNavigation).Where(x => x.TipoVehiculoNavigation.Tipo.Equals(tipoVehiculo)), "Placa", "Placa");
            return View(envio);
        }

       /// <summary>
       /// Edita un envio
       /// </summary>
       /// <param name="id"></param>
       /// <param name="envio"></param>
       /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Envio envio)
        {
            if (id != envio.NumeroGuia)
            {
                return NotFound();
            }

            if (envio.FechaEntrega <= envio.FechaRegistro)
            {
                return UnprocessableEntity("La fecha de entrega debe ser mayor a la del registro");
            }


            if (ModelState.IsValid)
            {
                _context.Update(envio);
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
        public async Task<IActionResult> Delete(string id, int tipo)
        {
            if (id == null || _context.Envios == null)
            {
                return NotFound();
            }
            if (tipo != Maritimo && tipo != Terrestre)
            {
                return BadRequest("El tipo de envio es incorrecto");
            }
            ViewData["TipoEnvio"] = tipo == Maritimo ? "Maritimo" : "Terrestre";

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

        /// <summary>
        /// Elimina Un registro
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
