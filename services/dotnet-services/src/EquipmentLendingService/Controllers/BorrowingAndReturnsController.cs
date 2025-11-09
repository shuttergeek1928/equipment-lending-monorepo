using EquipmentLendingDotnetServices.Data;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentLendingService.Controllers
{
    [ApiController]
    [Route("api/")]
    public class BorrowingAndReturnsController : ControllerBase
    {

        private readonly EquipmentLendingDBContext _context;

        public BorrowingAndReturnsController(EquipmentLendingDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string Get()
        {
            return "Api is working";
        }

        [HttpGet]
        [Route("requests")]
        public IActionResult GetAllRequests()
        {
            var requests = _context.BorrowingsAndReturns.ToList();
            return requests.Any() ? Ok(requests) : NotFound("No requests found.");
        }
    }
}
