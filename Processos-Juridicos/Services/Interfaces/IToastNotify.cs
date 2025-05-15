namespace Processos_Juridicos.Services.Interfaces;

public interface IToastNotify
{
    Task Sucesso(string msg);
    Task Error(string msg);
    Task Info(string msg);
    Task Alert(string msg);
    Task Warning(string msg);
}
