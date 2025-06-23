namespace Processos_Juridicos.Services.Interfaces;

public interface IToastNotify
{
    public void Sucesso(string msg);
    public void Error(string msg);
    public void Info(string msg);
    public void Alert(string msg);
    public void Warning(string msg);
}
