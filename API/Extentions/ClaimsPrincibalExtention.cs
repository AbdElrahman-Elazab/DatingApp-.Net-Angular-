using System.Security.Claims;

namespace API.Extentions;
public static class ClaimsPrincibalExtention
{
    public static string GetMemberId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("Cannot get memberId from token");
    }
}