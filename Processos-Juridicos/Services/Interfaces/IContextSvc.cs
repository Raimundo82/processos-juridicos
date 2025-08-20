namespace Processos_Juridicos.Services.Interfaces;

public interface IContextSvc
{
    public IMilitarySecuritySvc MilitarySecurity { get; }
    public IUnitSvc Units { get; }
    public IHarmedOrCasualtySvc Casualties { get; }
}
