using System.DirectoryServices.Protocols;
using System.Net;

using Processos_Juridicos.Configuration;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services.Ldap;

public class LdapConnSvc(LdapConfiguration configuration) : ILdapConnSvc
{
    private readonly LdapConfiguration _configuration = configuration;
    private LdapConnection? _ldapConnection;
    private bool _disposed = false;


    public LdapConnection GetConnection()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_ldapConnection != null)
        {
            try
            {
                TimeSpan _ = _ldapConnection.Timeout;
                return _ldapConnection;
            }
            catch (ObjectDisposedException)
            {
                _ldapConnection = null;
            }
        }

        LdapDirectoryIdentifier ldapDirectoryIdentifier = new(
            _configuration.Url,
            int.Parse(_configuration.Port),
            fullyQualifiedDnsHostName: false,
            connectionless: false);

        NetworkCredential networkCredential = new(_configuration.Username, _configuration.Password);

        _ldapConnection = new LdapConnection(ldapDirectoryIdentifier, networkCredential, AuthType.Basic)
        {
            Timeout = TimeSpan.FromSeconds(10),
        };
        _ldapConnection.SessionOptions.ProtocolVersion = 3;
        _ldapConnection.SessionOptions.SecureSocketLayer = true;

        try
        {
            _ldapConnection.Bind();
            return _ldapConnection;
        }
        catch (LdapException ex)
        {
            _ldapConnection?.Dispose();
            _ldapConnection = null;
            throw new InvalidOperationException($"Failed to connect to LDAP server at {_configuration.Url}:{_configuration.Port}", ex);
        }
    }

    public LdapConnection CreateConnection()
    {
        var ldapDirectoryIdentifier = new LdapDirectoryIdentifier(
            _configuration.Url,
            int.Parse(_configuration.Port),
            fullyQualifiedDnsHostName: false,
            connectionless: false);

        var networkCredential = new NetworkCredential(_configuration.Username, _configuration.Password);

        var conn = new LdapConnection(ldapDirectoryIdentifier, networkCredential, AuthType.Basic)
        {
            Timeout = TimeSpan.FromSeconds(10),
        };

        conn.SessionOptions.ProtocolVersion = 3;
        conn.SessionOptions.SecureSocketLayer = true;
        conn.Bind();

        return conn;
    }


    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _ldapConnection?.Dispose();
        _ldapConnection = null;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

}
