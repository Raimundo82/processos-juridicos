using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services.DomainData;

public class SentenceSvc(AppDbContext context) : ISentenceSvc
{
    private readonly AppDbContext _context = context;

    public async Task<SentenceDto> CreateSentence(SentenceDto sentence)
    {
        Sentence sentenceEntity = Mapper.MapToSentence(sentence);

        _context.Sentences.Add(sentenceEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToSentenceDto(sentenceEntity);
    }

    public async Task<bool> DeleteSentence(int? id)
    {
        Sentence? sentence = await _context.Sentences.FindAsync(id);
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
        Sentence existing = await _context.Sentences.FindAsync(sentence.SentenceId)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        Mapper.MapToSentence(sentence, existing);
        await _context.SaveChangesAsync();

        return Mapper.MapToSentenceDto(existing);
    }

    public async Task<IEnumerable<SentenceDto>> GetAllSentences()
    {
        List<Sentence> sentences = await _context.Sentences.AsNoTracking().ToListAsync();
        return Mapper.MapToToSentenceDtoEnum(sentences);
    }

    public async Task<SentenceDto> GetSentenceById(int? id)
    {
        Sentence sentence = await _context.Sentences.AsNoTracking().FirstOrDefaultAsync(a => a.SentenceId == id)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        return Mapper.MapToSentenceDto(sentence);
    }
}
