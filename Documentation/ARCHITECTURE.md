# Project Architecture 🏗️

This document describes the technical architecture of **Processos Jurídicos**.
The goal is to provide a clear overview of the system’s structure, technologies, and data flow, emphasizing its use of ASP.NET Core MVC and SQL Server.

---

## 🖼️ Overview

The system employs a traditional **Model-View-Controller (MVC)** pattern for its structure, providing a clear separation of concerns between data, business logic, and presentation.

---

## ⚙️ Technologies

* **Framework/Platform**
    * **ASP.NET Core MVC 8.0** (The core web application framework).
    * **.NET 8.0** (The underlying runtime).
* **Database**
    * **SQL Server** (Relational Database Management System).
    * **Entity Framework Core (EF Core)** (ORM for data interaction).

### Frontend Libraries

The client-side experience relies on the following standard libraries and frameworks for styling, interactivity, and enhanced controls:

| Library | Purpose |
| :--- | :--- |
| **Bootstrap** | Core framework for **responsive design** and styling of the application's UI components. |
| **jQuery** | A fast, small, and feature-rich JavaScript library for **DOM manipulation** and handling AJAX calls. |
| **jQuery Validation** | Client-side form **validation**. |
| **jQuery Validation Unobtrusive** | Integrates form validation seamlessly with **ASP.NET Core's model annotations**. |
| **DataTables** | Advanced table manipulation for **sorting, searching, and pagination** of large datasets. |
| **Select2** | Custom controls for enhancing standard HTML `<select>` elements, enabling **searchable dropdowns**. |
| **Bootstrap Icons** | Collection of **icons** for UI elements. |
| **Font Awesome** | Additional collection of scalable **vector icons**. |
| **Fontsource** | Local management of web **fonts** (@fontsource/prosto-one, @fontsource/roboto). |

---

## 📄 Document Generation

The system integrates **iText 7** library to generate PDF documents directly from HTML and CSS templates.  
This allows the application to:
- Produce official reports and process documentation in a standardized format.  
- Ensure consistent styling across documents by reusing existing CSS.  
- Automate the export of processes into secure, non-editable PDF files.  
- Support advanced features such as headers, footers, tables, and embedded images.  

By leveraging iText, the application guarantees compliance with legal requirements for document storage and distribution.
