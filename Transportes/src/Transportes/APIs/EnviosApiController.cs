using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transportes.Models;

namespace Transportes.APIs
{
    /// <summary>
    /// Api que administra las funciones de los envios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnviosApiController : ControllerBase
    {
        private readonly TransportesContext _context;

        public EnviosApiController(TransportesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los envios
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult GetEnvios()
        {
           var envios = _context.Envios.Include(e => e.CedulaClienteNavigation).Include(e => e.DestinoNavigation).Include(e => e.TipoProductoNavigation).Include(e => e.VehiculoNavigation);

            if (envios == null || !envios.Any())
            {
                return NoContent();
            }
            return Ok(envios);
        }

       /// <summary>
       /// obtiene un solo envio
       /// </summary>
       /// <param name="numeroGuia"></param>
       /// <returns></returns>
        [HttpGet("{numeroGuia}")]
        public async Task<IActionResult> Get(string numeroGuia)
        {
            var envio = await _context.Envios
                 .Include(e => e.CedulaClienteNavigation)
                 .Include(e => e.DestinoNavigation)
                 .Include(e => e.TipoProductoNavigation)
                 .Include(e => e.VehiculoNavigation)
                 .FirstOrDefaultAsync(m => m.NumeroGuia == numeroGuia);

            if (envio == null)
            {
                return NoContent();
            }
            return Ok(envio);
        }

        /// <summary>
        /// Guarda un envio
        /// </summary>
        /// <param name="value"></param>
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] Envio envio)
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
                return Ok();
            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Edita un envio
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="value"></param>
        [HttpPut("{numeroGuia}")]
        public async Task<IActionResult> Put(string numeroGuia, [FromBody] Envio envio)
        {
            if (numeroGuia != envio.NumeroGuia)
            {
                return NoContent();
            }

            if (envio.FechaEntrega <= envio.FechaRegistro)
            {
                return UnprocessableEntity("La fecha de entrega debe ser mayor a la del registro");
            }


            if (ModelState.IsValid)
            {
                _context.Update(envio);
                await _context.SaveChangesAsync();
                return Ok();

            }
            return UnprocessableEntity();
        }

        /// <summary>
        /// Elimina un envio
        /// </summary>
        /// <param name="numeroGuia"></param>
        [HttpDelete("{numeroGuia}")]
        public async Task<IActionResult> Delete(string numeroGuia)
        {
            if (_context.Envios == null)
            {
                return Problem("Entity set 'TransportesContext.Envios'  is null.");
            }
            var envio = await _context.Envios.FindAsync(numeroGuia);
            if (envio != null)
            {
                _context.Envios.Remove(envio);
            }
            else
            {
                return NoContent();
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool EnvioExists(string numeroGuia)
        {
            return (_context.Envios?.Any(e => e.NumeroGuia == numeroGuia)).GetValueOrDefault();
        }
    }
}
