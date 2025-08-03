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
    public class ProveedoresController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public ProveedoresController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/Proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
            => await _context.Proveedores.ToListAsync();

        // GET: api/web/Proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            return proveedor == null ? NotFound() : proveedor;
        }

        // POST: api/web/Proveedores
        [HttpPost]
        public async Task<ActionResult<Proveedor>> CreateProveedor(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.ProveedorID }, proveedor);
        }

        // PUT: api/web/Proveedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProveedor(int id, Proveedor proveedor)
        {
            if (id != proveedor.ProveedorID) return BadRequest();
            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}