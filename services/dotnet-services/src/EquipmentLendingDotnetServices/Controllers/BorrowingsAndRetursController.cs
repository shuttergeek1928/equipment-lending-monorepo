using EquipmentLendingDotnetServices.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquipmentLendingDotnetServices.Controllers
{
    [Route("api/")]
    [ApiController]
    public class BorrowingsAndRetursController(EquipmentLendingDBContext context) : ControllerBase
    {
        private readonly EquipmentLendingDBContext _context = context;

        [HttpGet]
        [Route("requests")]
        public IActionResult Get()
        {
            var equipments = _context.BorrowingsAndReturns.ToListAsync();
            return Ok(equipments);
        }
    }
}
