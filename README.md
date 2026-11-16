# Employee Leave Tracking System

A Windows desktop application for managing employee information, leave requests, overtime records, and reports.

## Tech Stack
- C#
- .NET 10 (Windows Forms)
- SQL Server
- Microsoft.Data.SqlClient

## Features
- Administrator login
- Employee management
- Leave and overtime records
- Reporting screens

## Run Locally
1. Open `PersonelIzinTakip.sln` in Visual Studio on Windows.
2. Make sure .NET 10 SDK and the Windows Desktop workload are installed.
3. Configure SQL Server locally. The source currently expects a local `SQLEXPRESS` instance and a database named `PersonelDB`.
4. Create the required tables (`Adminler`, employee, leave/overtime and report-related tables) before running.
5. Build and run the solution.

> This is an academic portfolio project. The database schema is not included in this repository, so a local database setup is required.
