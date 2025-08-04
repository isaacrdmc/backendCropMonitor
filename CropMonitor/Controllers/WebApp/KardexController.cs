using CropMonitor.Data;
using CropMonitor.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminWeb,InventoryManager")]
    public class KardexController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public KardexController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/Kardex/producto/5
        [HttpGet("producto/{productoId}")]
        public async Task<ActionResult<IEnumerable<Kardex>>> GetKardexProducto(int productoId)
        {
            return await _context.Kardex
                .Where(k => k.ProductoID == productoId)
                .OrderByDescending(k => k.Fecha)
                .Include(k => k.Producto)
                .ToListAsync();
        }

        // GET: api/web/Kardex/resumen
        [HttpGet("resumen")]
        public async Task<ActionResult<object>> GetResumenKardex()
        {
            var movimientosRecientes = await _context.Kardex
                .OrderByDescending(k => k.Fecha)
                .Take(10)
                .Include(k => k.Producto)
                .Select(k => new {
                    k.KardexID,
                    k.Fecha,
                    Producto = k.Producto.Nombre,
                    k.TipoMovimiento,
                    k.Cantidad,
                    k.CostoUnitario,
                    k.Saldo
                })
                .ToListAsync();

            var productosBajoStock = await _context.Productos
                .Where(p => p.Stock < 10)
                .Select(p => new {
                    p.ProductoID,
                    p.Nombre,
                    p.Stock,
                    p.Unidad
                })
                .ToListAsync();

            return new { movimientosRecientes, productosBajoStock };
        }
    }
}