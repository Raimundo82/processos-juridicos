using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests.ProcessTest;

internal static class ProcessTestData
{
    public static TheoryData<Process[]> BaseScenario { get; } =
            new TheoryData<Process[]>
            {
                    {
                        Array.Empty<Process>()
                    },
                    {
                        new[] {
                            new Process
                            {
                                Nuipm = $"0001/{DateTime.Now.Year}/UN01",
                                ProcessState = new ProcessState { StateName = "Em Edição" },
                                ProcessType = new ProcessType { Deadline=15, ProcessTypeName = "Tipo 1"},
                                CreatedAt = DateOnly.FromDateTime(DateTime.Today),
                                Unit = new Unit {UnitName ="Unidade 1", UnitCode = "UN01", UnitAcronym = "UN1"}
                            },
                        }
                    },
                    {
                        new[]
                        {
                            new Process
                            {
                                Nuipm = $"0001/{DateTime.Now.Year}/UN01",
                                CreatedAt = DateOnly.FromDateTime(DateTime.Today),
                                ProcessState = new ProcessState { StateName = "Em Validação" },
                                ProcessType = new ProcessType { Deadline=15, ProcessTypeName = "Tipo 1"},
                                Unit = new Unit {UnitName ="Unidade 1", UnitCode = "UN01", UnitAcronym = "UN1"}
                            },
                            new Process
                            {
                                Nuipm = $"0002/{DateTime.Now.Year}/UN02",
                                CreatedAt = DateOnly.FromDateTime(DateTime.Today),
                                ProcessState = new ProcessState { StateName = "Em Edição" },
                                ProcessType = new ProcessType { Deadline=15, ProcessTypeName = "Tipo 2" },
                                Unit = new Unit {UnitName ="Unidade 2", UnitCode = "UN02", UnitAcronym = "UN2"}
                            },
                        }
                    }
            };

}

