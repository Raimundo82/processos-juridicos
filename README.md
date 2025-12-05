# Processos Jurídicos

## 💡 About

**Processos Jurídicos** is an application that was created due to a growing concern with the performance and data security of the previous system which was implemented in sharepoint 2013 a now very outdated technology.

This new version of this aplication is a complete migration of the previous of the functionality and data to new technology, specifically ASP .NET CORE MVC and a SQL Server database. Functionally the application is very similar to the previous version with a few noteworthy improvements:

- Two factor authentication (2FA) due to the sensitive personal data being handled it was important to increase security around the handling and access of this information.
- Updated development technologies.
- Complete detachment from Sharepoint.
- Internal workflows.
- Increased performance.
- Improved accessibility and usability.

---

## 🕰️ History

- **Processos Jurídicos V1.0 (2016)**  
  Developed by the *Direção de Análise e Gestão de Informação (DAGI)*, this first version was integrated into the Marinha Portal Intranet using **SharePoint 2013** and **Nintex Workflows**.  
  Over time, the system grew to support around **3000 processes**. However, by May 2025, performance and data security issues emerged due to outdated technologies and the increasing volume of data.

- **Processos Jurídicos V2.0 (2025)**  
  In May 2025, DAGI initiated the development of a new version, migrating all existing data to a modern stack: **ASP .NET CORE MVC** and **SQL Server**.  
  While maintaining functional similarity to the previous version, V2.0 introduced significant improvements:
  - Two-factor authentication (2FA) for enhanced security  
  - Updated development technologies  
  - Independence from SharePoint, enabling service-oriented architecture or standalone operation  
  - Internal workflow system, eliminating Nintex licensing costs  
  - Improved performance  
  - Enhanced graphical interface, accessibility, and usability  

📌 **Production Release Date:** *14 November 2025*

---

## 📖 Documentation
All documentation can be found in the [`Documentation/`](./Documentation) folder.

- [Business Rules](./Documentation/BUSINESS_RULES.md)
- [Architecture Overview](./Documentation/ARCHITECTURE.md)
- [Installation Guide](./Documentation/SETUP.md)
- [Contributing Policy](./Documentation/CONTRIBUTING.md)
- [Testing Guide](./Documentation/TESTING.md)
- [Troubleshooting](./Documentation/TROUBLESHOOTING.md)
- [Database Mapping](./Documentation/DATABASE_MAPPING.md)