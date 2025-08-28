using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces;

public interface IGenericTableBuilderSvc
{
    public GenericTableModel Build<T>(
       string tableId,
       string controllerName,
       IEnumerable<T> data,
       List<string> headers,
       List<string> columnKeys,
       bool showActions,
       Func<T, GenericRowModel> rowSelector);
}
