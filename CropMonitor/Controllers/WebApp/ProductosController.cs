using CropMonitor.Data;
using CropMonitor.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminWeb,InventoryManager")]
    public class ProductosController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public ProductosController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/Productos
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
            => await _context.Productos.ToListAsync();

        // GET: api/web/Productos/5/con-kardex
        [HttpGet("{id}/con-kardex")]
        public async Task<ActionResult<object>> GetProductoConKardex(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Kardex)
                .FirstOrDefaultAsync(p => p.ProductoID == id);

            if (producto == null) return NotFound();

            return new
            {
                Producto = new
                {
                    producto.ProductoID,
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Precio,
                    producto.Stock,
                    producto.Unidad
                },
                Kardex = producto.Kardex
                    .OrderByDescending(k => k.Fecha)
                    .Select(k => new {
                        k.KardexID,
                        k.Fecha,
                        k.TipoMovimiento,
                        k.Cantidad,
                        k.CostoUnitario,
                        k.Promedio,
                        k.Debe,
                        k.Haber,
                        k.Saldo
                    })
            };
        }

        // POST: api/web/Productos/5/ajustar-inventario
        [HttpPost("{id}/ajustar-inventario")]
        public async Task<IActionResult> AjustarInventario(int id, [FromBody] AjusteInventarioDto ajuste)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            // Calcular nuevo saldo
            var ultimoKardex = await _context.Kardex
                .Where(k => k.ProductoID == id)
                .OrderByDescending(k => k.Fecha)
                .FirstOrDefaultAsync();

            decimal saldoAnterior = ultimoKardex?.Saldo ?? 0;
            decimal nuevoSaldo = saldoAnterior + (ajuste.Cantidad * (ajuste.CostoUnitario ?? producto.Precio ?? 0));

            // Registrar movimiento
            var movimiento = new Kardex
            {
                ProductoID = id,
                Fecha = DateTime.Now,
                TipoMovimiento = ajuste.Cantidad > 0 ? "ENTRADA" : "SALIDA",
                Cantidad = Math.Abs(ajuste.Cantidad),
                CostoUnitario = ajuste.CostoUnitario ?? producto.Precio ?? 0,
                Promedio = nuevoSaldo / (producto.Stock + ajuste.Cantidad),
                Debe = ajuste.Cantidad > 0 ? ajuste.Cantidad * (ajuste.CostoUnitario ?? producto.Precio ?? 0) : 0,
                Haber = ajuste.Cantidad < 0 ? Math.Abs(ajuste.Cantidad) * (ajuste.CostoUnitario ?? producto.Precio ?? 0) : 0,
                Saldo = nuevoSaldo,
            };

            // Actualizar stock
            producto.Stock += ajuste.Cantidad;

            _context.Kardex.Add(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class AjusteInventarioDto
        {
            [Required]
            public int Cantidad { get; set; }
            public decimal? CostoUnitario { get; set; }
            public string? Comentario { get; set; }
        }
    }
}