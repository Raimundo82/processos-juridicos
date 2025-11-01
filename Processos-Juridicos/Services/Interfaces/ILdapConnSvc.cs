using System.DirectoryServices.Protocols;

namespace Processos_Juridicos.Services.Interfaces;

public interface ILdapConnSvc : IDisposable
{
    public LdapConnection GetConnection();
}
