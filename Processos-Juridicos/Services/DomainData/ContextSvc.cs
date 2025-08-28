using Processos_Juridicos.Services.Interfaces.DomainData;

namespace Processos_Juridicos.Services.DomainData;

public class ContextSvc(
    IMilitarySecuritySvc militarySecurity,
    IUnitSvc units,
    IHarmedOrCasualtySvc casualties) : IContextSvc
{
    public IMilitarySecuritySvc MilitarySecurity { get; } = militarySecurity;
    public IUnitSvc Units { get; } = units;
    public IHarmedOrCasualtySvc Casualties { get; } = casualties;
}
