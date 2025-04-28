using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class EditOtherUsersHandler : AuthorizationHandler<EditOtherUsersRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditOtherUsersHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EditOtherUsersRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier); // Logged-in user ID
        var routeData = _httpContextAccessor.HttpContext?.Request.RouteValues;

        if (routeData != null && routeData.TryGetValue("id", out var userIdToEdit))
        {
            if (userId != null && userIdToEdit != null && userId.ToString() != userIdToEdit.ToString())
            {
                context.Succeed(requirement); // Allow editing others
            }
        }

        return Task.CompletedTask;
    }
}
