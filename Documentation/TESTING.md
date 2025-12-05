# Testing Guide 🧪

This document details the procedures for executing the **Integration Tests** within the **Processos Jurídicos** project.

---

## 🎯 Test Scope

The project employs **Integration Tests** to verify the correct interaction between the business logic, the Entity Framework Core data access layer, and the underlying database.

**Note:** These tests are configured to use EF Core's **`UseInMemoryDatabase`** provider. This ensures the tests are isolated, fast, and do not require a connection to an external **SQL Server** instance.

All integration tests are located within the dedicated project: **`Processos-Juridicos.Test`**.

---

## 🛠️ Prerequisites

To successfully run the integration tests, you must have the following running and configured:

1.  **.NET 8.0 SDK:** Required for building and executing the tests.
2.  **Project Dependencies Restored:** Ensure all NuGet packages are restored (`dotnet restore`).
3.  **Vitest 3.2.4:** Required for executing Javascript tests.

---

## ▶️ How to Run ASPNET Integration Tests

You can execute the test suite using either the **Visual Studio Test Explorer** or the **.NET Command Line Interface (CLI)**.

### Option 1: Using Visual Studio 2022

1.  **Open the Solution:** Load the `Processos-Juridicos.sln` solution file in **Visual Studio 2022**.
2.  **Open Test Explorer:** Navigate to **Test** > **Test Explorer**.
3.  **Build Solution:** Ensure the solution is built (`Build > Build Solution` or `Ctrl + Shift + B`) so the Test Explorer discovers the tests in the `Processos-Juridicos.Test` project.
4.  **Run Tests:**
    * Click **"Run All Tests"** in the Test Explorer window to execute the entire test suite.
5.  **Review Results:** Test results (Passed/Failed) will be displayed directly within the Test Explorer window.

### Option 2: Using the .NET Command Line Interface (CLI)

This method is ideal for quick execution or integration with scripts and pipelines.

1.  **Navigate to the Test Project:**
    Open your terminal or command prompt and change the directory to the test project folder:
    ```bash
    cd Processos-Juridicos.Test
    ```

2.  **Execute the Tests:**
    Run the following command to discover and execute all tests in the project:
    ```bash
    dotnet test
    ```
## ▶️ How to Run the Javascript Integration Tests
You can execute the test suite using **Vitest**.

### Using Vitest to test Javascript

1.  **Open the Solution's folder in a terminal:** Open a terminal window and travel to the top folder of the solution.
2.  **Run Tests:** write "npx vitest" or "npx vitest --coverage" in the console and execute.
3.  **Review Results:** Test results (Passed/Failed) will be displayed directly within the terminal window, in case the option --coverage was used, a folder with a complete coverage report will also be added to the project's folder which has more detailed information viewable by opening its files.