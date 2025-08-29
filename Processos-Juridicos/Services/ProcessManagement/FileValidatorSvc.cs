using System.Net;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services.ProcessManagement;

public class FileValidatorSvc(IProcessManagementSvc processManagement, IToastNotify toastNotify) : IFileValidatorSvc
{
    private readonly IProcessManagementSvc _processManagement = processManagement;
    private readonly IToastNotify _toastNotify = toastNotify;

    private readonly string[] _permittedFileExtensions = [".pdf", ".jpeg", ".png"];
    private readonly int _fileSizeLimit = 5 * 1024 * 1024;

    private readonly Dictionary<string, List<byte[]>> _fileSignatures = new()
    {
        { ".pdf", new List<byte[]> { "%PDF-"u8.ToArray() } },
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
            }
        },
        { ".png", new List<byte[]>
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
            }
        }
    };

    public async Task<bool> ValidateAndSaveFileAsync(int? processId, IFormFile file)
    {
        if (processId == null)
        {
            return false;
        }

        if (file == null || file.Length == 0)
        {
            _toastNotify.Error(GlobalTextManager.GetString("EmptyFileMessage"));
            return false;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        if (!ValidateFile(ext, ms, out var errorMessage))
        {
            _toastNotify.Error(errorMessage);
            return false;
        }

        var trustedName = WebUtility.HtmlEncode(file.FileName);

        ProcessFileDto fileDto = Mapper.MapToFilesDto(new ProcessFile
        {
            ProcessFileName = file.FileName,
            ProcessFileType = file.ContentType,
            ProcessFileContent = ms.ToArray(),
            ProcessFileTrustedName = trustedName,
            ProcessId = processId.Value
        });

        await _processManagement.ProcessFiles.CreateProcessFile(fileDto);
        return true;
    }

    private bool ValidateFile(string ext, MemoryStream ms, out string errorMessage)
    {
        if (!_permittedFileExtensions.Contains(ext))
        {
            errorMessage = GlobalTextManager.GetString("FileExtensionNotAllowedMessage");
            return false;
        }

        if (!VerifyFileSignatureCorrect(ms, ext))
        {
            errorMessage = GlobalTextManager.GetString("FileExtensionNotAllowedMessage");
            return false;
        }

        if (ms.Length > _fileSizeLimit)
        {
            errorMessage = GlobalTextManager.GetString("FileSizeTooLargeMessage");
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private bool VerifyFileSignatureCorrect(MemoryStream file, string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return false;
        }

        if (!_fileSignatures.TryGetValue(extension, out List<byte[]>? signatures) || signatures.Count == 0)
        {
            return false;
        }

        var maxSignatureLength = signatures.Max(sig => sig.Length);
        file.Position = 0;

        var headerBytes = new byte[maxSignatureLength];
        var bytesRead = file.Read(headerBytes, 0, maxSignatureLength);

        return signatures.Any(sig =>
            bytesRead >= sig.Length &&
            headerBytes.AsSpan(0, sig.Length).SequenceEqual(sig));
    }
}
