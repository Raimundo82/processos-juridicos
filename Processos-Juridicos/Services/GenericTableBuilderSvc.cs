using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class GenericTableBuilderSvc : IGenericTableBuilderSvc
{
    public GenericTableModel Build<T>(
        string tableId,
        string controllerName,
        IEnumerable<T> data,
        List<string> headers,
        List<string> columnKeys,
        bool showActions,
        Func<T, GenericRowModel> rowSelector)
    {
        return new GenericTableModel
        {
            TableId = tableId,
            Controller = controllerName,
            Headers = headers ?? [],
            ColumnKeys = columnKeys ?? [],
            ShowActions = showActions,
            Rows = data?.Select(rowSelector).ToList() ?? [],
        };
    }
}
