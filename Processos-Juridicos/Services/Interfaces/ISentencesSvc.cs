using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface ISentencesSvc
    {
        Task<IEnumerable<SentencesDTO>> getAllSentences();
        Task<Sentences> getSentencesById(int id);
        Task<Sentences> createSentence(Sentences sentence);
        Task<Sentences> editState(Sentences sentence);
        Task<bool> deleteState(int id);

    }
}
