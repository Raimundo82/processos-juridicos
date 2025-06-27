using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface ISentenceSvc
{
    public Task<IEnumerable<SentenceDto>> GetAllSentences();
    public Task<SentenceDto> GetSentenceById(int? id);
    public Task<SentenceDto> CreateSentence(SentenceDto sentence);
    public Task<SentenceDto> EditSentence(SentenceDto sentence);
    public Task<bool> DeleteSentence(int? id);
}
