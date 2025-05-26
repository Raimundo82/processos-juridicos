namespace Processos_Juridicos.Services.Interfaces;

public interface IToastNotify
{
    void Sucesso(string msg);
    void Error(string msg);
    void Info(string msg);
    void Alert(string msg);
    void Warning(string msg);
}   
