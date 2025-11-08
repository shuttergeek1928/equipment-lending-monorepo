using EquipmentLendingDotnetServices.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentLendingBackendService.Controllers
{
    [Route("api/dashboard/")]
    [ApiController]
    public class DashboardController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
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
    }
}
