using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class SentencesSvc : ISentencesSvc
    {

        private readonly AppDbContext _context;

        public SentencesSvc(AppDbContext context)
        {
            _context = context;
        }

        public Task<Sentences> createSentence(Sentences sentence)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteState(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Sentences> editState(Sentences sentence)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SentencesDTO>> getAllSentences()
        {
            var sentences = await _context.Sentences.ToListAsync();
            return Mapper.MapToToSentenceDtoEnum(sentences);
        }

        public Task<Sentences> getSentencesById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
