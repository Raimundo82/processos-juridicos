# Business Rules and Role Responsibilities

This document outlines the core business rules and the specific competencies/attributions associated with each user role within the **Processos Jurídicos** system.

---

## 👥 Roles and Competencies

The system defines three main groups of responsibility, mapped to a total of **five distinct roles** in the application's code base.

### ⚙️ System Roles (Code Definition)

The application utilizes the following five user roles:

* `OFICIAIS-INSTRUTORES`
* `COMANDO-UNIDADE`
* `DJ-AUTHORIZED`
* `DJ-UNAUTHORIZED`
* `SUPERADMIN`

---

### 1. Oficial Instrutor (OFICIAIS-INSTRUTORES)

| Competency | Description |
| :--- | :--- |
| **Process Initiation** | Start a new process, setting its initial status to **"Em Edição"** (In Editing). |
| **Data Entry** | Complete all required fields in the process form. |
| **Status Transition** | Transition the process status from "Em Edição" to **"Em Validação"** (In Validation). |

### 2. Com./Dir./Chefe da Unidade (COMANDO-UNIDADE)

| Competency | Description |
| :--- | :--- |
| **Status Transition (Dispatch)** | Transition the process between the **"Aberto"** (Open) and **"Despachado"** (Dispatched) statuses. |
| **Deadline Extension** | Grant extensions for the legal process deadlines (Prorrogação do prazo). |

### 3. DJ - Direção Jurídica (DJ-AUTHORIZED, DJ-UNAUTHORIZED, SUPERADMIN)

The DJ profile is split into three roles with differentiated access:

| Role | Responsibility | Competencies |
| :--- | :--- | :--- |
| **DJ-UNAUTHORIZED** | Corresponds to **Directors** of the DJ. | **Only Visualization:** Can view processes but cannot initiate any modification or status transition. |
| **DJ-AUTHORIZED** | Corresponds to the **Secretariat** of the DJ. | Can perform all actions listed below, excluding the exclusive permissions of the SuperAdmin role. |
| **SUPERADMIN** | The highest level, encompassing the **DJ-AUTHORIZED** role. | Has full permissions across the entire system. |

#### Competencies (DJ-AUTHORIZED/SUPERADMIN):

| Competency | Description |
| :--- | :--- |
| **Process Validation** | Review and validate the process. The process can be transitioned to **"Aberto"** (Open) or sent back to **"Em Edição"** (In Editing) for corrections. |
| **Generate NUIPM** | Generate the unique Process Identification Number (NUIPM) upon process opening. |
| **Manage Appeals** | Manage processes currently in the **"Em Recurso"** (In Appeal) status. |
| **Process Closure** | Close processes by transitioning their status to **"Fechado"** (Closed). |
| **User Management (Roles)** | Add or remove users from the **Oficial Instrutor**, **Com./Dir./Chefe da Unidade**, and **DJ** roles. |
| **Manual Process Upload** | Manual upload of legacy or external processes into the system. |
| **Statistical Analysis** | Generate and analyze statistical data and reports. |

### 3.1 Parameterization Competency

This competency is exclusive to **DJ-AUTHORIZED** and **SUPERADMIN** users:

| Competency | Description |
| :--- | :--- |
| **System Parameterization** | Access the **"Direção Jurídica"** menu option in the menubar to **add, edit, and remove** all parameterized data used across the system. |

---

## 📊 Process Status Flow (Summary)

This table summarizes the main state transitions as defined by the user roles:

| From Status | To Status | Triggering Role |
| :--- | :--- | :--- |
| **Em Edição** (In Editing) | **Em Validação** (In Validation) | Oficial Instrutor |
| **Em Validação** (In Validation) | **Aberto** (Open) / **Em Edição** (In Editing) | DJ (Authorized/SuperAdmin) |
| **Aberto** (Open) | **Despachado** (Dispatched) | Com./Dir./Chefe da Unidade |
| **Despachado** (Dispatched) | **Aberto** (Open) | Com./Dir./Chefe da Unidade |
| Any Status (Managed by DJ) | **Fechado** (Closed) | DJ (Authorized/SuperAdmin) |