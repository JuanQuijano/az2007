# Library App

## Description

This is a console-based application for managing a library's patrons and their book loans. It allows for searching patrons, viewing their details, and inspecting their loan history. The application is built using .NET with a clean architecture approach, separating concerns into domain, infrastructure, and presentation layers. Data is persisted in JSON files.

## Project Structure

*   `AccelerateDevGHCopilot.sln`
*   `src/`
    *   `Library.ApplicationCore/`
        *   `Library.ApplicationCore.csproj`
        *   `Entities/`
        *   `Enums/`
        *   `Interfaces/`
        *   `Services/`
    *   `Library.Console/`
        *   `appSettings.json`
        *   `CommonActions.cs`
        *   `ConsoleApp.cs`
        *   `ConsoleState.cs`
        *   `Library.Console.csproj`
        *   `Program.cs`
        *   `Json/`
    *   `Library.Infrastructure/`
        *   `Library.Infrastructure.csproj`
        *   `Data/`
*   `tests/`
    *   `UnitTests/`
        *   `LoanFactory.cs`
        *   `PatronFactory.cs`
        *   `UnitTests.csproj`
        *   `ApplicationCore/`

## Key Classes and Interfaces

*   **`ConsoleApp`**: The main class that drives the user interface and application flow in the console.
*   **`IPatronRepository` / `JsonPatronRepository`**: Interface and implementation for accessing patron data. `JsonPatronRepository` handles reading/writing patron information from JSON files.
*   **`ILoanRepository` / `JsonLoanRepository`**: Interface and implementation for accessing loan data. `JsonLoanRepository` handles reading/writing loan information from JSON files.
*   **`IPatronService` / `PatronService`**: Interface and implementation for business logic related to patrons, such as renewing memberships.
*   **`ILoanService` / `LoanService`**: Interface and implementation for business logic related to loans, such as processing returns.
*   **`JsonData`**: A helper class responsible for the low-level loading and saving of data to and from the JSON files on disk.

## Usage

To run the application, navigate to the console project directory and use the .NET CLI:

```sh
dotnet run --project src/Library.Console/Library.Console.csproj
```

## License

This project is licensed under the MIT License.