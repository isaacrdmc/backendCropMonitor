using CropMonitor.Data;
using CropMonitor.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public ContactoController(CropMonitorDbContext context) => _context = context;

        // POST: api/web/Contacto (Público)
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Contacto>> SendMessage(Contacto contacto)
        {
            contacto.FechaEnvio = DateTime.Now;
            _context.Contacto.Add(contacto);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Mensaje enviado exitosamente" });
        }

        // GET: api/web/Contacto (Admin)
        [HttpGet]
        [Authorize(Roles = "AdminWeb,SupportManager")]
        public async Task<ActionResult<IEnumerable<Contacto>>> GetMessages()
            => await _context.Contacto.OrderByDescending(c => c.FechaEnvio).ToListAsync();
    }
}