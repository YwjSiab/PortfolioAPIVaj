using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly PortfolioContext _context;

        public ProjectsController(PortfolioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectItem>>> GetProjects()
        {
            var projects = await _context.Projects
                .OrderBy(p => p.Id)
                .ToListAsync();

            return Ok(projects);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectItem>> PostProject(ProjectItem project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
        }
    }
}