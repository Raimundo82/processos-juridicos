namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IFileValidatorSvc
{
    public Task<int?> ValidateAndSaveFiles(int? processId, IFormFile file);
}
