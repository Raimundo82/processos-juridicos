using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class SentenceSvc(AppDbContext context) : ISentenceSvc
    {
        private readonly AppDbContext _context = context;

        public async Task<SentenceDto> CreateSentence(SentenceDto sentence)
        {
            var sentenceEntity = Mapper.MapToSentence(sentence);

            _context.Sentences.Add(sentenceEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToSentenceDto(sentenceEntity);
        }

        public async Task<bool> DeleteSentence(int id)
        {
            var sentence = await _context.Sentences.FindAsync(id);
            if (sentence == null) return false;

            _context.Sentences.Remove(sentence);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SentenceDto> EditSentence(SentenceDto sentence)
        {
            var sentenceEntity = Mapper.MapToSentence(sentence);
            _context.Sentences.Entry(sentenceEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Mapper.MapToSentenceDto(sentenceEntity);
        }

        public async Task<IEnumerable<SentenceDto>> GetAllSentences()
        {
            var sentences = await _context.Sentences.ToListAsync();
            return Mapper.MapToToSentenceDtoEnum(sentences);
        }

        public async Task<SentenceDto> GetSentenceById(int id)
        {
            var sentence = await _context.Sentences.FindAsync(id);

            if (sentence != null)
            {
                return Mapper.MapToSentenceDto(sentence);
            }

            throw new KeyNotFoundException();
        }
    }
}
