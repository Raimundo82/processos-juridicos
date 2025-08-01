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
                                Nuipm = "1234",
                                ProcessState = new ProcessState { StateName = "State 1" },
                                ProcessType = new ProcessType { Deadline=15, ProcessTypeName = "Tipo 1"}
                            },
                        }
                    },
                    {
                        new[]
                        {
                            new Process
                            {
                                Nuipm = "1234",
                                ProcessState = new ProcessState { StateName = "State 1" },
                                ProcessType = new ProcessType { Deadline=15, ProcessTypeName = "Tipo 1"}
                            },
                            new Process
                            {
                                Nuipm = "2345",
                                ProcessState = new ProcessState { StateName = "State 2" },
                                ProcessType = new ProcessType { Deadline=15, ProcessTypeName = "Tipo 2" }
                            },
                        }
                    }
            };

}

