using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests.TestHelpers;

internal static class TestUtilities
{
    public static State CreateState(string stateName)
    {
        return new State { StateName = stateName };
    }

    public static ProcessType CreateProcessType(string processTypeName, int deadline)
    {
        return new ProcessType { ProcessTypeName = processTypeName, Deadline = deadline };
    }

    public static void InitializeDbForProcessIntegrationTests(AppDbContext db)
    {

        if (!db.States.Any())
        {
            db.States.AddRange(GetSeedStates());
        }

        if (!db.ProcessTypes.Any())
        {
            db.ProcessTypes.AddRange(GetSeedProcessTypes());
        }

        if (db.ProcessFiles.Any())
        {
            db.ProcessFiles.RemoveRange(db.ProcessFiles);

            db.SaveChangesAsync();
        }

        db.SaveChanges();
    }

    public static List<State> GetSeedStates()
    {
        return
        [
            CreateState("1 - Em Edição"),
            CreateState("2 - Em Validação"),
            CreateState("3 - Aberto"),
            CreateState("4 - Despachado"),
            CreateState("5 - Em Recurso"),
            CreateState("6 - Fechado"),
        ];
    }

    public static List<ProcessType> GetSeedProcessTypes()
    {
        return
        [
            CreateProcessType("Averiguações - Militares", 15),
            CreateProcessType("Disciplinares - Militares", 35),
        ];
    }
}
