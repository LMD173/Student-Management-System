# Student Management System

A console-based C# application which simulates a student management system.

## 1. Usage

### 1.1 Setup

1. Download all the files
2. The Program looks for a `SMS_DB_PATH` environment variable to find the database file. If not found, it uses a default value of `./sms-data.db` which is in the root directory of the program's files (so no need to change anything for this step).
3. Make sure to have .NET installed (this program was made with .NET 8)

### 1.2 Run

1. Use NuGet to install dependencies (there are 3 - `ConsoleTables` for pretty-printing data in tables, `Microsoft.AspNetCore.Identity` for encryption and `Microsoft.Data.Sqlite` for SQLite-related functionality).
2. In the directory with the downloaded files, run `dotnet run` in your terminal. Alternatively, open an IDE/code editor and run the progrm.
3. You'll be asked for login details; the program gives you 3 security attempts before stopping (for demo purposes, you can always re-run the program). There are 2 users pre-loaded in the database:

| role  | email              | password           |
| ----- | ------------------ | ------------------ |
| admin | `admin@sms.com`    | `a8dHA73*!&£aHA4@` |
| user  | `john.doe@sms.com` | `9Hdb&263*2bd9d5$` |

3. You'll be greeted with a Menu. Admin users will have more options available (full read and write permissions). Users only have read access (apart from write access to change their own details).
4. When asked for input, the program will state the format needed (if any) and also tell you if a specific input can be left empty/skipped.
5. Exit any time with `Ctrl + C` or follow the Menu instructions to exit.

### 1.3 Student Menu

### 1.4 User Menu

### 1.5 Extra Information

-   There are 3 external dependencies - `ConsoleTables` for pretty-printing data in tables, `Microsoft.AspNetCore.Identity` for hashing and `Microsoft.Data.Sqlite` for SQLite-related functionality.
-   All data pre-loaded in the database was randomly generated.
