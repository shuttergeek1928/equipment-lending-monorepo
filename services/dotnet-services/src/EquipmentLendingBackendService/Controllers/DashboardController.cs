using EquipmentLendingDotnetServices.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EquipmentLendingBackendService.Controllers
{
    [Route("api/dashboard/")]
    [ApiController]
    public class DashboardController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet("debug-claims")]
        [Authorize] // or remove for public debugging
        public IActionResult DebugClaims()
        {
            var name = User.Identity?.Name;
            var usernameClaim = User.FindFirst("username")?.Value;
            var sub = User.FindFirst("sub")?.Value;
            
            var roles = User.Claims.Where(c => c.Type == "roles" || c.Type == ClaimTypes.Role || c.Type == "role")
                                   .Select(c => new { c.Type, c.Value }).ToList();
            
            var names = User.Claims.Where(c => c.Type == "nameidentifier" || c.Type == ClaimTypes.NameIdentifier)
                                   .Select(c => new { c.Type, c.Value }).ToList();

            var all = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(new { name, usernameClaim, sub, roles, all });
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllEquipments()
        {
            var equipments = _context.Equipments.ToList();
            return Ok(equipments);
        }

        [HttpGet]
        [Route("available")]
        public IActionResult GetAllAvailableEquipments(bool isAvailable)
        {
            var equipments = _context.Equipments.Where(e => e.IsAvailable && isAvailable).ToList();
            return Ok(equipments);
        }

        [HttpGet]
        [Route("category")]
        public IActionResult GetAllEquipments(string category)
        {
            var equipments = _context.Equipments.Where(e => e.Catgory.Equals(category)).ToList();
            return Ok(equipments);
        }

        [HttpGet]
        [Route("users")]
        public IActionResult getUsers()
        {
            var equipments = _context.Users.ToList();
            return Ok(equipments);
        }

    }
}
