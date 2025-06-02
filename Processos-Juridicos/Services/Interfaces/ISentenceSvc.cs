using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces;

public interface ISentenceSvc
{
    Task<IEnumerable<SentenceDto>> GetAllSentences();
    Task<SentenceDto> GetSentenceById(int id);
    Task<SentenceDto> CreateSentence(SentenceDto sentence);
    Task<SentenceDto> EditSentence(SentenceDto sentence);
    Task<bool> DeleteSentence(int id);

}
