namespace Processos_Juridicos.Services.Interfaces;

public interface ILegalReferenceSvc
{
    public ICrimeTypeSvc CrimeTypes { get; }
    public ISentenceSvc Sentences { get; }
    public IInfringementSvc Infringements { get; }
    public IAccidentTypeSvc AccidentTypes { get; }
    public IProcessTypeSvc ProcessTypes { get; }
}
