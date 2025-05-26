using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class SentenceSvc : ISentenceSvc
    {

        private readonly AppDbContext _context;

        public SentenceSvc(AppDbContext context)
        {
            _context = context;
        }

        public Task<Sentence> createSentence(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteState(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Sentence> editState(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SentenceDto>> getAllSentences()
        {
            var sentences = await _context.Sentences.ToListAsync();
            return Mapper.MapToToSentenceDtoEnum(sentences);
        }

        public Task<Sentence> getSentencesById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
