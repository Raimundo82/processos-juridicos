using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests.SectorTest;

public static class SectorTestData
{
    public static Sector CreateSector(string name, string code, bool enabled)
    {
        return new Sector
        {
            SectorName = name,
            SectorCode = code,
            Enable = enabled
        };
    }

    public static TheoryData<Sector[]> CreateScenario { get; } =
        [
            [],

            [
                CreateSector("Viação",  "VC",  true)
            ],

            [
                CreateSector("Viação",  "VC",  false),
                CreateSector("Serviço", "SRV", true)
            ],
        ];

    public static TheoryData<Sector[]> ListScenario { get; } =
        [
            [],

            [
                CreateSector("Viação",  "VC",  false),
                CreateSector("Serviço", "SRV", true)
            ]
        ];
}
