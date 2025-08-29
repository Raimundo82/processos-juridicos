using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IProcessViewDataSvc
{
    public Task PopulateForCreateAsync(ViewDataDictionary viewData);
    public Task PopulateForEditAsync(ViewDataDictionary viewData, int? processId);
}
