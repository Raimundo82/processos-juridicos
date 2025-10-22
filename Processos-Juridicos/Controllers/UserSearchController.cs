using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Controllers;

[ApiController]
[Route("api/directory")]
public class UserSearchController(IUserDataSvc userDataSvc) : ControllerBase
{
    private readonly IUserDataSvc _userDataSvc = userDataSvc;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Ok(Array.Empty<object>());
        }

        IReadOnlyList<UserDataModel>? results = await _userDataSvc.SearchUsersAsync(query.Trim());
        return Ok(results ?? []);
    }

    [HttpGet("resolve/{id}")]
    public async Task<IActionResult> Resolve(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Ok(new { found = false });
        }

        UserDataModel? result = await _userDataSvc.GetUserByNiiAsync(id.Trim());
        return result is null ? NotFound(new { found = false }) : Ok(result);
    }
}
