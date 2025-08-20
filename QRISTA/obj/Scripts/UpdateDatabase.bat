@echo off
echo Updating database for QRB Authorization system...

echo.
echo Building project...
dotnet build QRB.csproj

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Creating migration for Authorization system...
dotnet ef migrations add AddAuthorizationSystem --output-dir Migrations

if %ERRORLEVEL% NEQ 0 (
    echo Migration creation failed!
    pause
    exit /b 1
)

echo.
echo Updating database...
dotnet ef database update

if %ERRORLEVEL% NEQ 0 (
    echo Database update failed!
    echo.
    echo Please run the SQL script manually:
    echo Scripts\UpdateDatabaseForAuthorization.sql
    pause
    exit /b 1
)

echo.
echo Database updated successfully!
echo Authorization system is ready to use.
echo.
echo You can now access: http://localhost:5233/Authorization
pause
