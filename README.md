In Visual Studio, you can use the Package Manager Console to apply pending migrations to the database:

PM> Update-Database
Alternatively, you can apply pending migrations from a command prompt at your project directory:

> dotnet ef database update