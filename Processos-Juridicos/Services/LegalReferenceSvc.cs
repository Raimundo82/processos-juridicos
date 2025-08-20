using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class LegalReferenceSvc(
    ICrimeTypeSvc crimeTypes,
    ISentenceSvc sentences,
    IInfringementSvc infringements,
    IAccidentTypeSvc accidentTypes,
    IProcessTypeSvc processTypes) : ILegalReferenceSvc
{
    public ICrimeTypeSvc CrimeTypes { get; } = crimeTypes;
    public ISentenceSvc Sentences { get; } = sentences;
    public IInfringementSvc Infringements { get; } = infringements;
    public IAccidentTypeSvc AccidentTypes { get; } = accidentTypes;
    public IProcessTypeSvc ProcessTypes { get; } = processTypes;
}
