using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
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

        public async Task<SentenceDto> CreateSentence(SentenceDto sentence)
        {
            var existingSentence = await _context.Sentences.FirstOrDefaultAsync(s => s.SentenceName == sentence.SentenceName);
            if (existingSentence != null)
            {
                throw new InvalidOperationException($"Já existe uma sentença com o nome '{sentence.SentenceName}'.");
            }

            var sentenceEntity = Mapper.MapToSentence(sentence);

            _context.Sentences.Add(sentenceEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToSentenceDto(sentenceEntity);

        }

        public async Task<bool> DeleteSentence(int id)
        {
            var sentence = await _context.Sentences.FirstOrDefaultAsync(x => x.SentenceId == id);
            if (sentence == null)
            {
                return false;
            }

            _context.Sentences.Remove(sentence);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<SentenceDto> EditSentence(SentenceDto sentence)
        {
            var duplicateSentence = await _context.Sentences
               .Where(s => s.SentenceId == sentence.SentenceId && s.SentenceId != sentence.SentenceId)
               .FirstOrDefaultAsync();

            if (duplicateSentence != null)
            {
                throw new InvalidOperationException($"Já existe outra sentença com o nome '{sentence.SentenceName}'.");
            }

            var sentenceEntity = Mapper.MapToSentence(sentence);
            _context.Sentences.Entry(sentenceEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return sentence;

        }

        public async Task<IEnumerable<SentenceDto>> GetAllSentences()
        {
            var sentences = await _context.Sentences.ToListAsync();
            return Mapper.MapToToSentenceDtoEnum(sentences);
        }

        public async Task<SentenceDto> GetSentenceById(int id)
        {
            var sentence = await _context.Sentences.FirstOrDefaultAsync(s => s.SentenceId == id)
                        ?? throw new KeyNotFoundException($"A sentença com o ID {id} não foi encontrada");
            return Mapper.MapToSentenceDto(sentence);

        }
    }
}
