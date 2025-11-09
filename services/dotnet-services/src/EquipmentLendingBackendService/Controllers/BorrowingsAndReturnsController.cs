using EquipmentLendingBackendService.Models.DTOs;
using EquipmentLendingDotnetServices.Data;
using EquipmentLendingDotnetServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.Common;

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
        [Authorize]
        public IActionResult RequestBorrowing([FromBody] RequestEquipmentDTO requestEquipmentDTO)
        {
            // 1. Validate user claim
            var idViaNameClaim = User.Identity?.Name ?? User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(idViaNameClaim) || !Guid.TryParse(idViaNameClaim, out var userGuid))
                return Unauthorized("Invalid user claim.");

            // 2. Load user
            var user = _context.Users.FirstOrDefault(u => u.Id == userGuid);
            if (user == null)
                return Unauthorized("User not found.");

            // 3. Prepare values
            var requestedOn = requestEquipmentDTO.RequestedOn.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
            var returnDue = DateTime.UtcNow.AddDays(7);
            var newId = Guid.NewGuid();

            // 4. Compute next RequestId
            var lastRequestId = _context.BorrowingsAndReturns
                .OrderByDescending(r => r.RequestId)
                .Select(r => r.RequestId)
                .FirstOrDefault();
            var nextRequestId = lastRequestId + 1;

            // 5. Parameterized SQL with RETURNING RequestId
            var sql = @"
                INSERT INTO ""BorrowingsAndReturns"" (
                    ""Id"",
                    ""EquipmentId"",
                    ""RequesterId"",
                    ""RequestedQuantity"",
                    ""RequestedOn"",
                    ""ReturnDueDate"",
                    ""ReturnedOn"",
                    ""IsApproved"",
                    ""IsReturned"",
                    ""Notes"",
                    ""IsOverdueNotified"",
                    ""LastNotifiedAt"",
                    ""NotificationAttempts"",
                    ""RequestId""
                )
                VALUES (
                    @Id,
                    @EquipmentId,
                    @RequesterId,
                    @RequestedQuantity,
                    @RequestedOn,
                    @ReturnDueDate,
                    @ReturnedOn,
                    @IsApproved,
                    @IsReturned,
                    @Notes,
                    @IsOverdueNotified,
                    @LastNotifiedAt,
                    @NotificationAttempts,
                    @RequestId
                )
                RETURNING ""RequestId"";
            ";

            // 6. Execute scalar to get new RequestId
            var conn = _context.Database.GetDbConnection();
            int insertedRequestId;
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                DbParameter CreateParam(string name, object? value)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = name;
                    p.Value = value ?? DBNull.Value;
                    return p;
                }

                cmd.Parameters.Add(CreateParam("@Id", newId));
                cmd.Parameters.Add(CreateParam("@EquipmentId", requestEquipmentDTO.EquipmentId));
                cmd.Parameters.Add(CreateParam("@RequesterId", user.UserId));
                cmd.Parameters.Add(CreateParam("@RequestedQuantity", requestEquipmentDTO.Quantity));
                cmd.Parameters.Add(CreateParam("@RequestedOn", requestedOn));
                cmd.Parameters.Add(CreateParam("@ReturnDueDate", returnDue));
                cmd.Parameters.Add(CreateParam("@ReturnedOn", null));
                cmd.Parameters.Add(CreateParam("@IsApproved", false));
                cmd.Parameters.Add(CreateParam("@IsReturned", false));
                cmd.Parameters.Add(CreateParam("@Notes", "Initial request for equipment"));
                cmd.Parameters.Add(CreateParam("@IsOverdueNotified", false));
                cmd.Parameters.Add(CreateParam("@LastNotifiedAt", null));
                cmd.Parameters.Add(CreateParam("@NotificationAttempts", 0));
                cmd.Parameters.Add(CreateParam("@RequestId", nextRequestId));

                var scalar = cmd.ExecuteScalar();
                if (scalar == null || scalar == DBNull.Value)
                    return StatusCode(500, "Failed to retrieve inserted RequestId.");

                insertedRequestId = Convert.ToInt32(scalar);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }

            // 7. Build return object reflecting inserted row
            var createdRequest = new BorrowingsAndReturns
            {
                Id = newId,
                EquipmentId = requestEquipmentDTO.EquipmentId,
                RequesterId = user.UserId,
                RequestedQuantity = requestEquipmentDTO.Quantity,
                RequestedOn = requestedOn,
                ReturnDueDate = returnDue,
                ReturnedOn = null,
                IsApproved = false,
                IsReturned = false,
                Notes = "Initial request for equipment",
                IsOverdueNotified = false,
                LastNotifiedAt = null,
                NotificationAttempts = 0,
                RequestId = insertedRequestId
            };

            // 8. Return created request
            return Ok(createdRequest);
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
