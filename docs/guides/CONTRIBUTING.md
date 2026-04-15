# Contributing Guide 🤝

## 📌 How to Initiate a Contribution
All contributions should start with a **formal Issue** to track the work.

1. **Open an Issue:** Describe the bug, fix, or feature you plan to implement.  
2. **Start Development:** Once the Issue is created, you may create a branch and start codin

---

## 🔧 Development Workflow

Follow this standard workflow for all code contributions:

### 1. Branching & Changes

- **Create a new branch** for your task (follow Conventional Commits naming):
    ```bash
    git checkout -b fix/brief-description
    ```
- **Implement your changes** focusing on a single, atomic task.

### 2. Commit & Push

- **Commit your changes** using **[Conventional Commits](https://www.conventionalcommits.org/)**
    ```bash
    git commit -m "fix: correct database connection string"
    ```
- **Push your branch** and open a **Pull Request (PR)** against **master**, linking the corresponding Issue.

---

## ✨ Code Standards and Quality Checks

### Code Standards
- Adhere strictly to **ASP.NET Core MVC** and **C#** best practices.
- Ensure all logic is contained within the correct layers (MVC separation of concerns).
- Keep code **modular** and maintainable.

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