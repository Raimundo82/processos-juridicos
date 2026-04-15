# Testing Guide 🧪

This document describes how to run **Integration Tests** in the **Processos Jurídicos** project.

All integration tests are located in the **`Processos-Juridicos.Test`** project and use EF Core's **`UseInMemoryDatabase`** provider, making them isolated and fast without needing a real SQL Server instance.

---

## ▶️ Running ASP.NET Integration Tests

You can run the tests using **Visual Studio** or the **.NET CLI**.

### Option 1: Visual Studio 2022

1. **Open the solution** `Processos-Juridicos.sln` in Visual Studio 2022.
2. **Open Test Explorer**: Go to `Test` > `Test Explorer`.
3. **Build the solution** (`Build > Build Solution` or `Ctrl + Shift + B`) to discover all tests.
4. **Run Tests**: Click **"Run All Tests"**.
5. **Review Results**: Passed/Failed results are displayed in Test Explorer.

### Option 2: .NET CLI

1. **Navigate to the test project folder**:
```bash
cd Processos-Juridicos.Test
```

2. **Run all tests**:
```bash
dotnet test
```

---


## ▶️ Running JavaScript Integration Tests (Vitest)

1. Open a terminal at the solution root folder.
2. Execute the tests:
```bash
npx vitest
# or for coverage report
npx vitest --coverage
```

3. **Review Results**: 

Test results are displayed in the terminal. If --coverage is used, a coverage report folder will be created in the project.