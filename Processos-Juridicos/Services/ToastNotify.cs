using NToastNotify;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class ToastNotify : IToastNotify
{
    private readonly IToastNotification _toastNotification;

    public ToastNotify(IToastNotification toastNotification)
    {
        _toastNotification = toastNotification;
    }

    public async Task Alert(string msg)
    {
        _toastNotification.AddAlertToastMessage(msg, new ToastrOptions 
        { 
            Title = "Alerta"
           
        });
    }

    public async Task Error(string msg)
    {
        _toastNotification.AddErrorToastMessage(msg, new ToastrOptions
        {
            Title = "Erro"

        });
    }

    public async Task Info(string msg)
    {
        _toastNotification.AddInfoToastMessage(msg, new ToastrOptions
        {
            Title = "Informação"

        });
    }

    public async Task Sucesso(string msg)
    {
        _toastNotification.AddSuccessToastMessage(msg, new ToastrOptions
        {
            Title = "Sucesso"

        });
    }

    public async Task Warning(string msg)
    {
        _toastNotification.AddWarningToastMessage(msg, new ToastrOptions
        {
            Title = "Aviso"

        });
    }
}
