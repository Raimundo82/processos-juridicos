using System.DirectoryServices.Protocols;
using System.Net;

using Processos_Juridicos.Configuration;

namespace Processos_Juridicos.Services.Ldap;

public class LdapConnSvc(LdapConfiguration configuration)
{
    private readonly LdapConfiguration _ldapConfiguration = configuration;
    private readonly LdapDirectoryIdentifier _ldapDirectoryIdentifier = new(string.Format("{0}:{1}", configuration?.Url, configuration?.Port));

    public LdapConnection GetConnection()
    {
        return GetConnection(_ldapConfiguration?.Username, _ldapConfiguration?.Password);
    }

    public LdapConnection GetConnection(string? username, string? password)
    {
        return new LdapConnection(_ldapDirectoryIdentifier, new NetworkCredential(username, password));
    }

    public LdapConfiguration GetLdapConfiguration()
    {
        return _ldapConfiguration;
    }
}
