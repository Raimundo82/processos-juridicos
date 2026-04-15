# Installation Guide 🛠️

## 🚀 Getting Started (Dev Container – Recommended)

This project is ready to run using **VS Code Dev Containers**, providing a fully configured development environment with **.NET 10** and **SQL Server**.

---

## 📁 Dev Container Overview

The development environment is defined in `.devcontainer/` and includes:

- **Primary service**: `app`
- **.NET SDK**: 10.0
- **Database**: SQL Server (via Docker Compose)
- **Workspace folder**: `/workspace`
- **Post-create setup**:
  - Container image build
  - SQL Server startup
  - Workspace mount
  - Required VS Code extensions installation

---

## ✅ Requirements

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

---

## ▶️ Setup

### 1. Clone the repository

```bash
git clone https://devops-01.marinha.pt/marinha-si/Processos-Juridicos.git
cd Processos-Juridicos
```

### 2. Configure environment variables

- Create a `.env` file inside the `.devcontainer` folder based on the example file:

```bash
cp .devcontainer/.env.example .devcontainer/.env
```

- Update the variables in `.env` with the values provided by the **Administrator** or **project team members**

This file contains sensitive settings such as database credentials, LDAP, and Keycloak secrets.

### 3. Open in Dev Container

- Open the project folder in **Visual Studio Code**
- When prompted, click **"Reopen in Container"**
- Or press `F1` and run: **Dev Containers: Reopen in Container**

VS Code will automatically build the container, start all required services, and configure the environment.

### 4. Run the application

```bash
dotnet run
```

The application will be available at http://localhost on the configured port (surely will be **`8080`**).

---

## 🖥️ Local Setup (Optional)

If you prefer running the application locally:

### 1. Clone and Restore Dependencies

```bash
git clone https://devops-01.marinha.pt/marinha-si/Processos-Juridicos.git
cd Processos-Juridicos
dotnet restore
```

### 2. Configure Connection Strings via .NET User Secrets

Sensitive data like database credentials, LDAP, and Keycloak secrets are **not stored in configuration files**. Use **.NET User Secrets** for local development.

1. **Request credentials** from the Project Administrator or team lead.

2. **Initialize User Secrets** (if not already done):

```bash
dotnet user-secrets init
```

3. **Set the secrets** using the values provided:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YourServer;Database=ProcessosJuridicosDb;User Id=YourUser;Password=YourPassword;"
dotnet user-secrets set "Ldap__Marinha__Password" "YourLdapPassword"
dotnet user-secrets set "Keycloak__credentials__secret" "YourKeycloakSecret"
```

### 3. Apply Database Migrations

```bash
dotnet ef database update
```

### 4. **Run the Application**
```bash
dotnet run
```

## 🤝 Contributing

Contributions are welcome. Please follow the guidelines in [Contributing Policy](CONTRIBUTING.md).