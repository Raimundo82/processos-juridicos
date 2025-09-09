using System.Security.Claims;

namespace Processos_Juridicos.Utilities;

public static class ClaimsPrincipalExtensions
{
    public static bool IsInstrutor(this ClaimsPrincipal user)
    {
        return user.IsInRole("OFICIAIS-INSTRUTORES");
    }

    public static bool IsComando(this ClaimsPrincipal user)
    {
        return user.IsInRole("COMANDO-UNIDADE");
    }

    public static bool IsDj(this ClaimsPrincipal user)
    {
        return user.IsInRole("DJ-UNAUTHORIZED") ||
        user.IsInRole("DJ-AUTHORIZED") ||
        user.IsInRole("SUPERADMIN");
    }

    public static bool IsDjAdministration(this ClaimsPrincipal user)
    {
        return user.IsInRole("DJ-AUTHORIZED") ||
        user.IsInRole("SUPERADMIN");
    }

    public static bool IsSuperAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("SUPERADMIN");
    }
}
