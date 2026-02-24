using AdminService.Services;
using GUIApps.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Commented out for now to ensure connectivity first, but should be enabled later
public class UsersController : ControllerBase
{
    private readonly KeycloakService _keycloakService;

    public UsersController(KeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserInfo>>> GetUsers()
    {
        try
        {
            var users = await _keycloakService.GetUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
