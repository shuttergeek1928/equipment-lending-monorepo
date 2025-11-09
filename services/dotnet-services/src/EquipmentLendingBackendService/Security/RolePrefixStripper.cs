using System.Security.Claims;

namespace EquipmentLendingBackendService.Security
{
    public class RolePrefixStripper : Microsoft.AspNetCore.Authentication.IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity id)
            {
                var existing = id.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var incomingRoles = id.FindAll("roles").Select(c => c.Value).ToList();

                foreach (var r in incomingRoles)
                {
                    var cleaned = r.StartsWith("ROLE_") ? r.Substring(5) : r;
                    if (!existing.Contains(cleaned))
                        id.AddClaim(new Claim(ClaimTypes.Role, cleaned));
                }
            }
            return Task.FromResult(principal);
        }
    }
}
