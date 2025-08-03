using CropMonitor.Data;
using CropMonitor.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    public class GuiaCultivoController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public GuiaCultivoController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/GuiaCultivo (Público)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GuiaCultivo>>> GetGuias()
            => await _context.GuiaCultivo
                .Include(g => g.Cultivo)
                .ToListAsync();

        // POST: api/web/GuiaCultivo
        [HttpPost]
        [Authorize(Roles = "AdminWeb,ContentEditor")]
        public async Task<ActionResult<GuiaCultivo>> CreateGuia(GuiaCultivo guia)
        {
            guia.FechaPublicacion = DateTime.Now;
            _context.GuiaCultivo.Add(guia);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGuias), guia);
        }
    }
}