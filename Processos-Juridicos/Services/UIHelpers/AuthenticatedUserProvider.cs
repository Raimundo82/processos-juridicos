using System.Security.Claims;
using System.Text.Json;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.UIHelpers;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Services.UIHelpers;

public class AuthenticatedUserProvider(IHttpContextAccessor http, IUserDataSvc userDataSvc) : IAuthenticatedUserProvider
{
    private const string SessionCacheKey = "AuthenticatedUserCache";

    public async Task<AuthenticatedUserViewModel> GetAsync()
    {
        HttpContext? context = http.HttpContext;
        if (context is null)
        {
            return new AuthenticatedUserViewModel();
        }

        ClaimsPrincipal user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return new AuthenticatedUserViewModel();
        }

        if (context.Session.TryGetValue(SessionCacheKey, out var sessionBytes))
        {
            try
            {
                AuthenticatedUserViewModel? cachedUser = JsonSerializer.Deserialize<AuthenticatedUserViewModel>(sessionBytes);
                if (cachedUser is not null)
                {
                    return cachedUser;
                }
            }
            catch (JsonException)
            {
                // Corrupted data in the session — ignore and search for fresh data.
            }
        }

        var nii = user.FindFirst("preferred_username")?.Value
            ?? (user.FindFirst("nii")?.Value is { Length: > 0 } rawNii ? $"m{rawNii}" : string.Empty);

        var displayName = user.FindFirst("display_name")?.Value
            ?? user.FindFirst(ClaimTypes.Name)?.Value
            ?? "Utilizador";

        var photo = "/images/default-avatar.webp";
        if (!string.IsNullOrEmpty(nii))
        {
            UserDataModel? userData = await userDataSvc.FetchUserPhoto(nii);
            if (userData?.UserPhoto is not null)
            {
                photo = $"data:image/png;base64,{Convert.ToBase64String(userData.UserPhoto)}";
            }
        }

        var viewModel = new AuthenticatedUserViewModel
        {
            IsAuthenticated = true,
            DisplayName = displayName,
            Nii = nii,
            Photo = photo
        };

        var serialized = JsonSerializer.SerializeToUtf8Bytes(viewModel);
        context.Session.Set(SessionCacheKey, serialized);

        return viewModel;
    }
}
