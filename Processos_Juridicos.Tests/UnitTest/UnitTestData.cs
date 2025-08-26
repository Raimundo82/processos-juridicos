namespace Processos_Juridicos.Tests.UnitTest;

public class UnitTest
{

    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Acronym { get; set; }
    public bool IsEnabled { get; set; }
}


public static class UnitTestData
{
    public static TheoryData<UnitTest[]> ListScenario { get; } =
        new TheoryData<UnitTest[]>
        {
            {
                Array.Empty<UnitTest>()
            },
            {
                new[]
                {
                    new UnitTest {
                        Name ="Direcção de Pessoal",
                        Code = "01",
                        Acronym = "DP",
                        IsEnabled = false,
                    },
                    new UnitTest {
                        Name ="Direcção de Análise e Gestão da Informação",
                        Code = "02",
                        Acronym = "DAGI",
                        IsEnabled = true,
                    }
                }
            }
        };


    public static TheoryData<UnitTest[], UnitTest> EditScenario { get; } =
        new TheoryData<UnitTest[], UnitTest>
        {
            {
                new[]
                {
                    new UnitTest {
                        Name      = "Direcção de Pessoal",
                        Code      = "01",
                        Acronym   = "DP",
                        IsEnabled = true
                    },
                    new UnitTest {
                        Name      = "Direcção 2",
                        Code      = "02",
                        Acronym   = "D2",
                        IsEnabled = true
                    }
                },

                new UnitTest {
                    Name      = "Nome Atualizado",
                    Code      = "Codigo Atualizado",
                    Acronym   = "Acronimo Atualizado",
                    IsEnabled = true
                }
            },
            {
                new[]
                {
                    new UnitTest {
                        Name      = "Direcção de Pessoal",
                        Code      = "01",
                        Acronym   = "DP",
                        IsEnabled = true
                    }
                },

                new UnitTest {
                    Name      = string.Empty,
                    Code      = string.Empty,
                    Acronym   = string.Empty,
                    IsEnabled = true
                }
            }
        };

    public static TheoryData<UnitTest[], UnitTest> DeleteScenario { get; } =
    new TheoryData<UnitTest[], UnitTest>
    {
        {
           new[]
            {
                new UnitTest {
                    Name      = "Direcção de Pessoal",
                    Code      = "01",
                    Acronym   = "DP",
                    IsEnabled = true
                },
                new UnitTest {
                    Name      = "Direcção 2",
                    Code      = "02",
                    Acronym   = "D2",
                    IsEnabled = true
                }
            },
            new UnitTest {
                Name      = "Direcção de Pessoal",
                Code      = "01",
                Acronym   = "DP",
                IsEnabled = true
            }
        },
        {
            new[]
            {
                new UnitTest {
                    Name      = "Direcção de Pessoal",
                    Code      = "01",
                    Acronym   = "DP",
                    IsEnabled = true
                },
                new UnitTest {
                    Name      = "Direcção 2",
                    Code      = "02",
                    Acronym   = "D2",
                    IsEnabled = true
                }
            },
            new UnitTest {
                Name      = string.Empty,
                Code      = string.Empty,
                Acronym   = string.Empty,
                IsEnabled = true
            }
        }
    };

    public static TheoryData<UnitTest[]> CreateScenario { get; } =
           new TheoryData<UnitTest[]>
           {
                    {
                       Array.Empty<UnitTest>()
                    },
                    {
                        new[]
                        {
                            new UnitTest {
                                Name ="Direcção de Pessoal",
                                Code = "01",
                                Acronym = "DP",
                                IsEnabled = true
                            }
                        }
                    },
                    {
                        new[]
                        {
                            new UnitTest {
                                Name ="Direcção de Pessoal",
                                Code = "01",
                                Acronym = "DP",
                                IsEnabled = true,
                             },
                            new UnitTest {
                                Name ="Direcção de Análise e Gestão da Informação",
                                Code = "02",
                                Acronym = "DAGI",
                                IsEnabled = true
                             }
                        }
                    }
           };

}

