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
    public class ComprasController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public ComprasController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
            => await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
                .ToListAsync();

        // POST: api/web/Compras
        [HttpPost]
        public async Task<ActionResult<Compra>> CreateCompra(Compra compra)
        {
            compra.FechaCompra = DateTime.Now;
            compra.Total = compra.DetalleCompras?.Sum(dc => dc.Cantidad * dc.CostoUnitario) ?? 0;

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompras), compra);
        }
    }
}