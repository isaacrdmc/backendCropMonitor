using CropMonitor.Data;
using CropMonitor.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminWeb,SalesManager")]
    public class VentasController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public VentasController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/Ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
            => await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVentas)
                .ThenInclude(dv => dv.Producto)
                .ToListAsync();

        // POST: api/web/Ventas
        [HttpPost]
        public async Task<ActionResult<Venta>> CreateVenta(Venta venta)
        {
            venta.FechaVenta = DateTime.Now;
            venta.Total = venta.DetalleVentas?.Sum(dv => dv.Cantidad * dv.PrecioVenta) ?? 0;

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVentas), venta);
        }
    }
}