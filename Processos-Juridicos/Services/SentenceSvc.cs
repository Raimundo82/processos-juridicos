using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class SentenceSvc(AppDbContext context) : ISentenceSvc
{
    private readonly AppDbContext _context = context;

    public async Task<SentenceDto> CreateSentence(SentenceDto sentence)
    {
        Sentence sentenceEntity = Mapper.MapToSentence(sentence);

        _ = _context.Sentences.Add(sentenceEntity);
        _ = await _context.SaveChangesAsync();
        return Mapper.MapToSentenceDto(sentenceEntity);
    }

    public async Task<bool> DeleteSentence(int? id)
    {
        Sentence? sentence = await _context.Sentences.FindAsync(id);
        if (sentence == null)
        {
            return false;
        }

        _ = _context.Sentences.Remove(sentence);
        _ = await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SentenceDto> EditSentence(SentenceDto sentence)
    {
        Sentence sentenceEntity = Mapper.MapToSentence(sentence);
        _context.Sentences.Entry(sentenceEntity).State = EntityState.Modified;

        _ = await _context.SaveChangesAsync();
        return Mapper.MapToSentenceDto(sentenceEntity);
    }

    public async Task<IEnumerable<SentenceDto>> GetAllSentences()
    {
        List<Sentence> sentences = await _context.Sentences.ToListAsync();
        return Mapper.MapToToSentenceDtoEnum(sentences);
    }

    public async Task<SentenceDto> GetSentenceById(int? id)
    {
        Sentence? sentence = await _context.Sentences.FindAsync(id);

        return sentence != null ? Mapper.MapToSentenceDto(sentence) : throw new EntityNotFoundException("Sentence not found");
    }
}
