using CropMonitor.Data;
using CropMonitor.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public BlogController(CropMonitorDbContext context) => _context = context;

        // GET: api/web/Blog (Público)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Blog>>> GetPosts()
            => await _context.Blog
                .Include(b => b.ComentariosBlog)
                .OrderByDescending(b => b.FechaPublicacion)
                .ToListAsync();

        // POST: api/web/Blog
        [HttpPost]
        [Authorize(Roles = "AdminWeb,ContentEditor")]
        public async Task<ActionResult<Blog>> CreatePost(Blog post)
        {
            post.FechaPublicacion = DateTime.Now;
            _context.Blog.Add(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPosts), post);
        }
    }
}