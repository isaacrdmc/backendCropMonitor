using CropMonitor.Data;
using CropMonitor.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    public class FAQController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public FAQController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/FAQ (Público)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FAQ>>> GetFAQs()
            => await _context.FAQ.ToListAsync();

        // POST: api/web/FAQ
        [HttpPost]
        [Authorize(Roles = "AdminWeb,ContentEditor")]
        public async Task<ActionResult<FAQ>> CreateFAQ(FAQ faq)
        {
            _context.FAQ.Add(faq);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFAQs), faq);
        }
    }
}