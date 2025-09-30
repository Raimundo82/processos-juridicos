using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.Ldap;

namespace Processos_Juridicos.Controllers;

[ApiController]
[Route("api/directory")]
public class UserSearchController(ILdapUserSvc directory) : ControllerBase
{
    private readonly ILdapUserSvc _directory = directory;

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string query, [FromQuery] int take = 25)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Ok(Array.Empty<object>());
        }

        IReadOnlyList<UserDataModel> results = _directory.SearchUsers(query.Trim(), Math.Clamp(take, 1, 100));
        return Ok(results.Select(u => new
        {

            displayName = u.DisplayName,
            samAccountName = u.UserName,
            userPrincipalName = u.FullUser,
            email = u.Email,
            department = u.Unit,
            employeeId = u.Nii
        }));

    }

    [HttpGet("resolve/{id}")]
    public IActionResult Resolve(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Ok(new { found = false });
        }

        try
        {
            UserDataModel user = _directory.GetUserDataByNii(id.Trim());
            return Ok(new
            {
                found = true,
                displayName = user.DisplayName,
                username = user.UserName,
                nii = user.Nii,
                fullUser = user.FullUser,
                unit = user.Unit,
                photoBase64 = user.PhotoBase64,
                groups = user.Groups
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Utilizador não encontrado" });
        }
    }
}
