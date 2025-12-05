# Installation Guide 🛠️

This document provides step-by-step instructions for setting up and running the **Processos Jurídicos** application. We recommend using the **Development Container** for the quickest and most consistent setup.

---

## 🚀 Option 1: Development Container (Recommended)

Using a Dev Container standardizes the development environment across all team members, eliminating setup discrepancies and local dependency conflicts.

### 📋 Prerequisites (Dev Container)
* **1. Visual Studio Code:** The IDE for development.
* **2. Docker Desktop:** Required to run the containers (available for Windows, macOS, and Linux).
* **3. VS Code Extensions:** Ensure you have the `Dev Containers` extension installed (`ms-vscode-remote.remote-containers`).

### ▶️ Setup Steps

1.  **Clone the Repository:**
    ```bash
    git clone https://devops-01.marinha.pt/marinha-si/Processos-Juridicos.git
    cd Processos-Juridicos
    ```

2.  **Open in Container:**
    * Open **Visual Studio Code** in the cloned directory.
    * A notification should appear asking: **"The folder contains a Dev Container configuration file. Reopen in Container?"**
    * Click **"Reopen in Container"**.

    > If the notification doesn't appear, open the Command Palette (`Ctrl+Shift+P` or `F1`) and run the command: `Dev Containers: Reopen in Container`.

3.  **Wait for Build:**
    VS Code will now build the container image (or download it, if pre-built) and start the development environment. This process automatically performs several critical steps defined in `.devcontainer/devcontainer.json` and associated files:
    * Installs the necessary **.NET 8.0 SDK**.
    * Installs required VS Code extensions.
    * Starts the dependent services (e.g., the **SQL Server** container).
    * Runs the `postCreateCommand` to automatically **restore dependencies** (`dotnet restore`) and **apply EF Core migrations** (`dotnet ef database update`).

4.  **Run the Application:**
    * Once the container is ready, open the terminal inside VS Code (which is now connected to the container).
    * Execute the run command:
        ```bash
        dotnet run
        ```
    * The application will be accessible via the exposed port (usually `http://localhost:5000` or `http://localhost:7000`).

---

## 🖥️ Option 2: Local Installation (Alternative)

This option requires manual installation and configuration of all prerequisites on your local machine.

### 📋 Prerequisites (Local)

* **1. .NET 8.0 SDK:** The core framework.
    * [Download .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* **2. SQL Server:** The relational database system.
* **3. Visual Studio (2022 recommended) or VS Code.**
* **4. Git:** For cloning the repository.

### ▶️ Setup Steps (Local)

1.  **Clone and Restore Dependencies:**
    ```bash
    git clone https://devops-01.marinha.pt/marinha-si/Processos-Juridicos.git
    cd Processos-Juridicos
    dotnet restore
    ```

### 2. Configure Connection String via Secrets

For security reasons, sensitive data like the database connection string is **not stored** directly in configuration files but managed via **.NET User Secrets** during development.

1.  **Request Credentials:** Contact the **Project Administrator** or **Team Lead** to obtain the required connection string and credentials for the development SQL Server environment.

2.  **Initialize User Secrets:**
    If you haven't already initialized secrets for this project, run this command in the project directory:
    ```bash
    dotnet user-secrets init
    ```

3.  **Set the Secret:**
    Use the following command format, replacing the placeholder connection string with the string provided by the Administrator:
    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YourServer;Database=ProcessosJuridicosDb;User Id=YourUser;Password=YourPassword;"
    ```

> **Note:** The application will automatically load this secret during development, overriding any value in `appsettings.json`.

3.  **Apply Database Migrations:**
    Run the EF Core command line tool to create and update the database schema:
    ```bash
    dotnet ef database update
    ```

4.  **Run the Application:**
    Execute the application from the command line or your IDE:
    ```bash
    dotnet run
    ```
    The application will start and should be accessible via `http://localhost` on the configured port.