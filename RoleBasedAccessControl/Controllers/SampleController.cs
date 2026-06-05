using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RoleBasedAccessControl.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet("general")]
        public IActionResult GeneralInfo()
        {
            return Ok("This is general information accessible to everyone.");
        }

        [Authorize()]
        [HttpGet("user")]
        public IActionResult UserInfo()
        {
            return Ok("If you see this you are logged in");
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("manager")]
        public IActionResult ManagerInfo()
        {
            return Ok("If you see this you are a manager and have access");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminInfo() 
        {
            return Ok("If you see this you are an admin and have access");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(
            [FromServices] UserManager<IdentityUser> userManager,
            [FromBody] AssignRoleRequest request)
        {
            var email = request.Email;
            var role = request.Role;
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return NotFound("User not found");

            var result = await userManager.AddToRoleAsync(user, role);
            return result.Succeeded ? Ok($"Role '{role}' assigned to {email}")
                                    : BadRequest(result.Errors);
        }
    }

    public record AssignRoleRequest(string Email, string Role);
}
