# Student Management System

A console-based C# application which simulates a student management system.

## 1. Usage

### 1.1 Setup

1. Download all the files
2. The Program looks for a `SMS_DB_PATH` environment variable to find the database file. If not found, it uses a default value of `./sms-data.db` which is in the root directory of the program's files (so no need to change anything for this step).
3. Make sure to have .NET installed (this program was made with .NET 8)

### 1.2 Run

1. Use NuGet to install dependencies (there are 3 - `ConsoleTables` for pretty-printing data in tables, `Microsoft.AspNetCore.Identity` for hashing and `Microsoft.Data.Sqlite` for SQLite-related functionality).
2. In the folder with the downloaded files, run `dotnet run` in your terminal. Alternatively, open an IDE/code editor and run the progrm.
3. You will be asked for login details; the program gives you 3 security attempts before stopping (for demo purposes, you can always re-run the program). There are 2 users pre-loaded in the database:

| role  | email              | password           |
| ----- | ------------------ | ------------------ |
| admin | `admin@sms.com`    | `a8dHA73*!&£aHA4@` |
| user  | `john.doe@sms.com` | `9Hdb&263*2bd9d5$` |

3. You will be greeted with a Menu. Admin users will have more options available (full read and write permissions). Users have more limited access.
4. When asked for input, the program will state the format needed (if any) and also tell you if a specific input can be left empty/skipped.
5. Exit any time with `Ctrl + C` or follow the Menu instructions to exit.

### 1.3 Student Menu

1. View all students - get a list of all students currently in the database and their details.
2. Search for a student by ID - you will be prompted to enter a student's ID; on success, you can view that specific student's details.
3. Search for a student by name or postcode - you will be prompted to enter a first name, last name, and postcode. All three options are optional and they act like filters to search the data. Leaving one blank will exclude it from the filter. The first and last names will be filtered to be an exact match whereas the postcode can be a partial match.
4. Add a new student - you will be prompted to provide a new student's details; on success, they will be added to the database and you can start searching for them by ID, name, or postcode.
5. Modify a student's details - you will be prompted to provide new details for the student, each of which is optional. On success, the student's data will be updated on the database.
6. Delete a student - you will be prompted for the student's ID; on success, their record will be removed from the database and they will not be searchable anymore.
7. Manage users - please see section `1.4 User Menu` below.
8. Exit - exit the program.

### 1.4 User Menu

1. View all users - get a list of all users currently in the database.
2. Update your details - you will be prompted to provide new details for your account, each of which is optional. Modifying the password will result in a new salt and a new hash (this means your password is jumbled up in an unreadable format to prevent any possible attackers from using it to access the system).
3. Update another user's details - you will be prompted to provide new details for the user, each of which is optional. The same password behaviour as point `2` occurs.
4. Add a new user - you will be prompted to provide a new user's details; on success, they will be added to the database and you will be able to use their details to log in.
5. Delete a user - you will be prompted for the user's ID; on success, they will be deleted from the database and you will no longer be able to log in with their account. If attempting to delete the last admin user, the program will warn you that this is not possible.
6. Exit - exit the User Wizard back to the main Student menu.

### 1.5 Extra Information

-   There are 3 external dependencies - `ConsoleTables` for pretty-printing data in tables, `Microsoft.AspNetCore.Identity` for hashing and `Microsoft.Data.Sqlite` for SQLite-related functionality.
-   All data pre-loaded in the database was randomly generated.
-   If you wish to use your own database file (using the `SMS_DB_PATH` environment variable), it must have the following schema:
    ```sql
    CREATE TABLE "Student" (
        "id"	INTEGER NOT NULL UNIQUE,
        "first_name"	TEXT,
        "last_name"	TEXT,
        "height"	REAL,
        "date_of_birth"	TEXT,
        "postcode"	TEXT,
        "address_line"	TEXT,
        "contact_phone_number"	TEXT,
        "contact_email"	TEXT,
        PRIMARY KEY("id" AUTOINCREMENT)
    );
    ```
    ```sql
    CREATE TABLE "User" (
        "id"	INTEGER NOT NULL UNIQUE,
        "email"	TEXT NOT NULL UNIQUE,
        "password"	TEXT NOT NULL,
        "role"	TEXT NOT NULL,
        "salt"	TEXT NOT NULL,
        PRIMARY KEY("id" AUTOINCREMENT)
    );
    ```
