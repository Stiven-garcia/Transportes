using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Transportes.Models;

namespace Transportes.Controllers
{
    /// <summary>
    /// Administra los vehiculos del sistema
    /// </summary>
    public class VehiculosController : Controller
    {
        private readonly TransportesContext _context;

        public VehiculosController(TransportesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Carga la lista de los vehiculos del sistema
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var transportesContext = _context.Vehiculos.Include(v => v.TipoVehiculoNavigation);
            ViewData["TipoVehiculo"] = new SelectList(_context.TipoVehiculos, "IdTipoVehiculo", "Tipo");
            return View(await transportesContext.ToListAsync());
        }

        /// <summary>
        /// Filtra los vehiculos del sistema
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Search(int? tipoVehiculo, string placa)
        {

            var vehiculo = _context.Vehiculos.Where(x => (!tipoVehiculo.HasValue || x.TipoVehiculo== tipoVehiculo)
             && (string.IsNullOrEmpty(placa)|| x.Placa.Contains(placa)));

            var vehiculoList = await vehiculo.ToListAsync();

            if (vehiculoList == null || !vehiculoList.Any())
            {
                return NotFound();
            }
            ViewData["TipoVehiculo"] = new SelectList(_context.TipoVehiculos, "IdTipoVehiculo", "Tipo");
            return View(nameof(Index), vehiculoList);
        }

       /// <summary>
       /// Obtiene la vista que permite crear vehiculos en el sistema
       /// </summary>
       /// <returns></returns>
        public IActionResult Create()
        {
            ViewData["TipoVehiculo"] = new SelectList(_context.TipoVehiculos, "IdTipoVehiculo", "Tipo");
            return View();
        }

        /// <summary>
        /// Permite crear un vehiculo
        /// </summary>
        /// <param name="vehiculo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehiculo vehiculo)
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(vehiculo.Placa) && vehiculo.TipoVehiculo != null)
            {
                string placa = @"^[A-Za-z]{3}\d{3}$";
                string flota = @"^[A-Za-z]{3}\d{4}[A-Za-z]$";
               
                bool isCamion = Regex.IsMatch(vehiculo.Placa, placa);
                bool isFlota = Regex.IsMatch(vehiculo.Placa, flota);
                if ((vehiculo.TipoVehiculo == 2 && isCamion)
                || (vehiculo.TipoVehiculo == 1 && isFlota))
                {
                    isValid = true;
                }
               
            }

            if (isValid)
            {
                _context.Add(vehiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return UnprocessableEntity("El formato de la placa del vehiculo no corresponde a la requerida");
        }

        /// <summary>
        /// Obtiene la vista que permite eliminar un vehiculo
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(string placa)
        {
            if (placa == null || _context.Vehiculos == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .Include(v => v.TipoVehiculoNavigation)
                .FirstOrDefaultAsync(m => m.Placa == placa);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        /// <summary>
        /// Elimina un vehiculo del sistema
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string placa)
        {
            if (_context.Vehiculos == null)
            {
                return Problem("Contexto es nulo");
            }
            var vehiculo = await _context.Vehiculos.FindAsync(placa);
            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
