using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests.IntegrationTests.ProcessTypeTest;

public static class ProcessTypeTestData
{
    public static ProcessType CreateProcessType(string name, int deadline)
    {
        return new ProcessType
        {
            ProcessTypeName = name,
            Deadline = deadline
        };
    }

    public static TheoryData<ProcessType[]> CreateScenario { get; } =
        [
            [],

            [
                CreateProcessType("Averiguações - Militares", 15)
            ],

            [
                CreateProcessType("Averiguações - Militares", 15),
                CreateProcessType("Acidentes em serviço", 180)
            ],
        ];

    public static TheoryData<ProcessType[]> ListScenario { get; } =
        [
            [],

            [
                CreateProcessType("Averiguações - Militares", 15),
                CreateProcessType("Acidentes em serviço", 180)
            ]
        ];
}
