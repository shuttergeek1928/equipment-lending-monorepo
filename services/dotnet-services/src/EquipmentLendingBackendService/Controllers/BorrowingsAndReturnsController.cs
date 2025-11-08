using EquipmentLendingBackendService.Models.DTOs;
using EquipmentLendingDotnetServices.Data;
using EquipmentLendingDotnetServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentLendingBackendService.Controllers
{
    [ApiController]
    [Route("api/")]
    public class BorrowingsAndReturnsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BorrowingsAndReturnsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string CheckApi()
        {
            return "BorrowingsAndReturns API is running.";
        }

        [HttpGet]
        [Route("borrowings")]
        public IActionResult GetAllBorrowings()
        {
            var borrowings = _context.BorrowingsAndReturns.ToList();
            return Ok(borrowings);
        }

        [HttpPost]
        [Route("request")]
        public IActionResult RequestBorrowing([FromBody] RequestEquipmentDTO requestEquipmentDTO)
        {
            var request = new BorrowingsAndReturns
            {
                EquipmentId = requestEquipmentDTO.EquipmentId,
                RequesterId = requestEquipmentDTO.RequesterId,
                RequestedQuantity = requestEquipmentDTO.Quantity,
                RequestedOn = requestEquipmentDTO.RequestedOn.ToDateTime(TimeOnly.MinValue).ToUniversalTime(),
                IsApproved = false
            };

            _context.BorrowingsAndReturns.Add(request);
            _context.SaveChanges();

            return Ok(request);
        }

        [HttpPut]
        [Route("approve/{requestId}")]
        public IActionResult ApproveRequest(int requestId)
        {

            var request = _context.BorrowingsAndReturns.FirstOrDefault(r => r.RequestId == requestId);

            if (request == null)
            {
                return NotFound($"Request with ID {requestId} not found.");
            }

            request.IsApproved = true;

            _context.SaveChanges();

            return Ok(request);
        }

        [HttpPut]
        [Route("return/{requestId}")]
        public IActionResult ReturnEquipment(int requestId)
        {

            var request = _context.BorrowingsAndReturns.FirstOrDefault(r => r.RequestId == requestId);

            if (request == null)
            {
                return NotFound($"Request with ID {requestId} not found.");
            }

            request.ReturnedOn = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok(request);
        }
    }
}
