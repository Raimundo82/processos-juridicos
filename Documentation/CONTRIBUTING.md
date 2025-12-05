# Contributing Guide 🤝

## 📌 How to Initiate a Contribution
All code contributions **should** start with a formal **Issue** to ensure the work is tracked and documented.

1.  **Open an Issue:** Clearly describe the bug, fix, or feature you intend to work on. This serves as the formal record for the task.
2.  **Begin Development:** Once the Issue is created, you may proceed with the development workflow below.

---

## 🔧 Development Workflow

Follow this standard workflow for all code contributions:

### 1. Preparation
1.  **Fork the Repository:** Click the *Fork* button on the Gitea instance to create your personal copy.
2.  **Clone Locally:**
    ```bash
    git clone https://devops-01.marinha.pt/marinha-si/Processos-Juridicos.git
    cd Processos-Juridicos
    ```

### 2. Branching & Changes
3.  **Create a New Branch:**
    Your branch name **must** follow the Conventional Commits specification, typically including the type and a short description.
    ```bash
    # Example: fix/database-conn-error
    git checkout -b fix/brief-description
    ```
4.  **Implement Your Changes:**
    - Follow **ASP.NET Core** and **C#** conventions.
    - Focus on a single, atomic task per branch/PR.

### 3. Commit & Push
5.  **Commit Your Changes:**
    Commit messages **must** follow the **[Conventional Commits](https://www.conventionalcommits.org/)** standard. This enables clear history tracking.
    ```bash
    # Example: fix: correct database connection string
    git commit -m "feat: descriptive message following the standard"
    ```
6.  **Push Your Branch:**
    ```bash
    git push origin fix/brief-description
    ```
7.  **Submit a Pull Request (PR):**
    Open a PR against the `main` branch of the official repository, linking it directly to the created Issue.

---

## ✨ Code Standards and Quality Checks

### Code Standards
- Adhere strictly to **ASP.NET Core MVC** and **C#** best practices.
- Ensure all logic is contained within the correct layers (MVC separation of concerns).
- Keep code **modular** and maintainable.

### Commit Messages
All commit messages are **required** to follow the **[Conventional Commits](https://www.conventionalcommits.org/)** standard.

| Type | Purpose | Example |
| :--- | :--- | :--- |
| `feat` | A new feature or enhancement. | `feat: add PDF export option for case files` |
| `fix` | A bug fix. | `fix: resolve null reference error on form submission` |
| `docs` | Documentation changes only. | `docs: update CONTRIBUTING guide` |
| `chore` | Maintenance tasks, like dependency updates. | `chore: update .NET 8.0 packages` |

---

## ✅ Pull Request Checklist

Before merging, all PRs **must** satisfy the following requirements:

- [x] **Issue Linked:** The PR is linked to a created Issue.
- [x] **Tests Included:** New features or critical fixes **must** include corresponding Unit and/or Integration tests.
- [x] **Local Build Verified:** Verified the application builds and runs locally without errors.
- [x] **Code Standards Followed:** Changes adhere to C# and ASP.NET Core conventions.
- [x] **Clear Commits:** All commits follow the **Conventional Commits** standard.
- [x] **CI/CD Passed:** All **Gitea Checks (Pipelines)** must pass successfully before the PR can be reviewed and merged.
- [x] **Documentation Updated:** Relevant documentation (e.g., `README.md`, `BUSINESS_RULES.md`) has been updated.

---

## 💬 Communication

Use the project's **Issue Tracker** for all formal communication regarding contributions, feature suggestions, and bug reports.