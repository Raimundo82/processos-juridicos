using System.Diagnostics;
using NToastNotify;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class ToastNotify(IToastNotification toastNotification) : IToastNotify
{
    private readonly IToastNotification _toastNotification = toastNotification;

    public void Alert(string msg)
    {
        _toastNotification.AddAlertToastMessage(msg, new ToastrOptions 
        { 
            Title = "Alerta"
           
        });
    }

    public void Error(string msg)
    {
        _toastNotification.AddErrorToastMessage(msg, new ToastrOptions
        {
            Title = "Erro"

        });
    }

    public void Info(string msg)
    {
        _toastNotification.AddInfoToastMessage(msg, new ToastrOptions
        {
            Title = "Informação"

        });
    }

    public void Sucesso(string msg)
    {
        _toastNotification.AddSuccessToastMessage(msg, new ToastrOptions
        {
            Title = "Sucesso"

        });
       
    }

    public void Warning(string msg)
    {
        _toastNotification.AddWarningToastMessage(msg, new ToastrOptions
        {
            Title = "Aviso"

        });
    }
}
