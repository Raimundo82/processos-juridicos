# Troubleshooting Guide 🛠️

This document provides step-by-step instructions for resolving common problems that may arise during this project while in a production environment.

## A user is unable to see any processes in the application

This project requires a user to be inserted either automatically by the project's worker or manually through the user interface in the User table of the database. If for some reason a user has been removed from the User table they will not be able to see any processes when they access the application.

## The application only shows a blank screen past the log-in stage

In this situation it's very likely that the application's SQL Server database is unreachable or authentication, the best way to diagnose the issue is to first try accessing the database in SQL Server with the project's username and password. If it's impossible to access the database with the project's credentials one of three situations may have happened:

-**The database server is down:** in this case the only thing that's necessary is for the server to be put back into operation and have its database services restored.
- **The user has an expired password:** in this case a renewal of the password is necessary, once renewed the project should function correctly once again.
- **The database server had its access configuration reset:** in some servers the configuration may reset for a variety of reasons making access through SQL server credentials impossible. To solve this particular variant of the problem someone with permissions should access the server and ensure that in the server's properties, under security, "Server authentication" is set to "SQL Server and Windows Authentication mode" instead of "Windows Authentication mode".
---
