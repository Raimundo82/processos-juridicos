namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IFileValidatorSvc
{
    public Task<int?> ValidateAndSaveFiles(int? processId, IFormFile file);

    public Task<bool> ValidateFile(IFormFile file);

    public Task<int?> SaveFile(int? processId, IFormFile file);
}
