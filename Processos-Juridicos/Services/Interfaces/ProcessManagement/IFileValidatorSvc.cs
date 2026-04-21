namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IFileValidatorSvc
{
    public Task<bool> ValidateAndSaveFiles(int? processId, IFormFile file);
}
