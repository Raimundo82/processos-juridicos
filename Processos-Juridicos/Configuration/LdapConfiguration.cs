namespace Processos_Juridicos.Configuration;

public class LdapConfiguration
{
    public string Url { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string BindDN { get; set; } = string.Empty;
    public string BaseDN { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
