using System.Security.Claims;

namespace TrainSyncAPI.Extensions;

public static class UserExtensions
{
    public static string GetClerkUserId(this ClaimsPrincipal user)
    {
        Console.WriteLine("[GetClerkUserId] Claims:");
        foreach (var claim in user.Claims)
            Console.WriteLine($" - {claim.Type}: {claim.Value}");
        return user.FindFirst("sub")?.Value
               ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? throw new UnauthorizedAccessException("Missing Clerk user ID.");
    }
}