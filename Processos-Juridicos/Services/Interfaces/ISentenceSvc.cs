using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces;

public interface ISentenceSvc
{
    Task<IEnumerable<SentenceDto>> getAllSentences();
    Task<Sentence> getSentencesById(int id);
    Task<Sentence> createSentence(Sentence sentence);
    Task<Sentence> editState(Sentence sentence);
    Task<bool> deleteState(int id);

}
