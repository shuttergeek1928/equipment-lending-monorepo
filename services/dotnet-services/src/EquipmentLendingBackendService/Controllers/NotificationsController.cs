using EquipmentLendingDotnetServices.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Expressions;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace EquipmentLendingBackendService.Controllers
{
    [Route("api/notifications/")]
    [ApiController]
    [AllowAnonymous]
    public class NotificationsController(ApplicationDbContext _context, INotificationSender _sender, HttpClient httpClient) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllNotifications()
        {
            var notifications = _context.Notifications.ToList();
            return notifications is null ? Ok(notifications) : NotFound();
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchNotification([FromQuery] string toEmail)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest("toEmail is required.");

            var api = $"http://localhost:8025/api/v2/search?kind=to&query={Uri.EscapeDataString(toEmail)}";
            using var response = await httpClient.GetAsync(api);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var body = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // Find "items" property case-insensitively
                if (!TryGetPropertyIgnoreCase(root, "items", out var itemsElement) || itemsElement.ValueKind != JsonValueKind.Array)
                {
                    // If there are no items, return an empty list
                    return Ok(Array.Empty<CleanedNotificationDto>());
                }

                var list = new List<CleanedNotificationDto>();
                foreach (var item in itemsElement.EnumerateArray())
                {
                    var id = GetFirstStringProperty(item, new[] { "id", "ID", "Id", "message", "Message" });

                    // From
                    string? from = null;
                    if (TryGetPropertyIgnoreCase(item, "from", out var fromEl))
                        from = ElementToContactString(fromEl);

                    // To - collect addresses
                    var toList = new List<string>();
                    if (TryGetPropertyIgnoreCase(item, "to", out var toEl))
                    {
                        if (toEl.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var t in toEl.EnumerateArray())
                            {
                                var s = ElementToContactString(t);
                                if (!string.IsNullOrEmpty(s)) toList.Add(s);
                            }
                        }
                        else
                        {
                            var s = ElementToContactString(toEl);
                            if (!string.IsNullOrEmpty(s)) toList.Add(s);
                        }
                    }

                    // Subject: try Content -> Headers -> Subject (often an array)
                    string? subject = null;
                    if (TryGetPropertyIgnoreCase(item, "content", out var contentEl))
                    {
                        if (TryGetPropertyIgnoreCase(contentEl, "headers", out var headersEl))
                        {
                            if (TryGetPropertyIgnoreCase(headersEl, "subject", out var subjEl))
                            {
                                if (subjEl.ValueKind == JsonValueKind.Array && subjEl.GetArrayLength() > 0)
                                    subject = subjEl[0].GetString();
                                else if (subjEl.ValueKind == JsonValueKind.String)
                                    subject = subjEl.GetString();
                            }
                        }

                        // Body: try common locations
                        subject ??= GetFirstStringProperty(contentEl, new[] { "subject", "Subject" });
                    }

                    string? bodyText = null;
                    // Try common content body properties
                    if (contentEl.ValueKind != JsonValueKind.Undefined)
                    {
                        bodyText = GetFirstStringProperty(contentEl, new[] { "body", "Body", "bodyPlain", "BodyPlain", "preview" });
                    }

                    // Fallbacks: sometimes body exists at top-level or under MIME
                    if (string.IsNullOrEmpty(bodyText))
                    {
                        bodyText = GetFirstStringProperty(item, new[] { "body", "Body" });
                    }

                    // Created/Date
                    DateTimeOffset? createdAt = null;
                    var createdStr = GetFirstStringProperty(item, new[] { "created", "Created", "date", "Date", "time", "Time" });
                    if (!string.IsNullOrEmpty(createdStr) && DateTimeOffset.TryParse(createdStr, out var dto))
                        createdAt = dto;

                    list.Add(new CleanedNotificationDto
                    {
                        Id = id ?? Guid.NewGuid().ToString(),
                        From = from,
                        To = toList,
                        Subject = subject,
                        Body = bodyText,
                        CreatedAt = createdAt
                    });
                }

                return Ok(list.OrderByDescending(l => l.CreatedAt));
            }
            catch (JsonException)
            {
                // Parsing failed - return raw response wrapped so caller can inspect
                return Ok(new { Raw = body });
            }
            catch (Exception ex)
            {
                // Unexpected errors - return a 500 with minimal info
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Failed to parse mailhog response", detail = ex.Message });
            }
        }

        [HttpPost]
        [Route("send")]
        [AllowAnonymous]
        public IActionResult SendNotification([FromQuery] int userId, [FromQuery] int requestId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var result = _sender.SendEmailAsync(user?.Email, "Test Notification...", "Test Notification Body .!");
            if (!result.IsCompleted)
                return NoContent();

            return Ok(result);
        }

        
        #region Helper types and methods

        private static bool TryGetPropertyIgnoreCase(JsonElement el, string name, out JsonElement value)
        {
            foreach (var prop in el.EnumerateObject())
            {
                if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = prop.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        private static string? GetFirstStringProperty(JsonElement el, string[] names)
        {
            foreach (var name in names)
            {
                if (TryGetPropertyIgnoreCase(el, name, out var v))
                {
                    if (v.ValueKind == JsonValueKind.String)
                        return v.GetString();
                    if (v.ValueKind == JsonValueKind.Array && v.GetArrayLength() > 0)
                    {
                        var first = v[0];
                        if (first.ValueKind == JsonValueKind.String) return first.GetString();
                        // if array of objects, try to stringify useful subprops
                        if (first.ValueKind == JsonValueKind.Object)
                        {
                            var s = ElementToContactString(first);
                            if (!string.IsNullOrEmpty(s)) return s;
                        }
                    }
                    if (v.ValueKind == JsonValueKind.Object)
                    {
                        // Try common fields inside the object
                        var candidate = GetFirstStringProperty(v, new[] { "body", "Body", "data", "text", "Text", "address", "Address", "mailbox" });
                        if (!string.IsNullOrEmpty(candidate)) return candidate;
                        // As a last resort, return raw JSON for the object
                        return v.GetRawText();
                    }
                    // other kinds -> try to get raw text
                    return v.GetRawText();
                }
            }
            return null;
        }

        private static string? ElementToContactString(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.String)
                return el.GetString();

            if (el.ValueKind == JsonValueKind.Object)
            {
                // Try common patterns: "address", "mailbox"+"domain", "Name", "name"
                if (TryGetPropertyIgnoreCase(el, "address", out var addrEl) && addrEl.ValueKind == JsonValueKind.String)
                    return addrEl.GetString();

                var mailbox = GetFirstStringProperty(el, new[] { "mailbox", "MailBox", "MailBox", "mail", "email", "Email", "name", "Name" });
                var domain = GetFirstStringProperty(el, new[] { "domain", "Domain" });

                if (!string.IsNullOrEmpty(mailbox) && !string.IsNullOrEmpty(domain))
                    return $"{mailbox}@{domain}";

                // If object contains simple string fields, return them concatenated
                var sbPieces = new List<string>();
                foreach (var prop in el.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.String)
                        sbPieces.Add($"{prop.Name}:{prop.Value.GetString()}");
                }
                if (sbPieces.Count > 0)
                    return string.Join(";", sbPieces);

                // fallback to raw JSON
                return el.GetRawText();
            }

            if (el.ValueKind == JsonValueKind.Array)
            {
                var addresses = new List<string>();
                foreach (var arrItem in el.EnumerateArray())
                {
                    var s = ElementToContactString(arrItem);
                    if (!string.IsNullOrEmpty(s)) addresses.Add(s);
                }
                return addresses.Count > 0 ? string.Join(", ", addresses) : null;
            }

            return null;
        }

        private class CleanedNotificationDto
        {
            public string Id { get; set; } = default!;
            public string? From { get; set; }
            public List<string> To { get; set; } = new();
            public string? Subject { get; set; }
            public string? Body { get; set; }
            public DateTimeOffset? CreatedAt { get; set; }
        }

        #endregion
    }
}
