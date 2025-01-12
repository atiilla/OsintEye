

## For the first time, you need to create the admin user

> dotnet run --project OsintEyeWeb.csproj -- create-admin

## For the first time, you need to create the newbie role

> dotnet run --project OsintEyeWeb.csproj -- create-role newbie

## For the first time, you need to initialize the database
```
Add-Migration InitialCreate
Update-Database
```