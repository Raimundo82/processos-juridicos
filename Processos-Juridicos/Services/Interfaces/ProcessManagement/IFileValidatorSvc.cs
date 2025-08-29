namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IFileValidatorSvc
{
    public Task<bool> ValidateAndSaveFileAsync(int? processId, IFormFile file);
}
