@echo off
REM =============================================================
REM Sales Quotation API - Database Setup Script
REM =============================================================
REM This script will setup your SQL Server database automatically
REM =============================================================

setlocal enabledelayedexpansion

echo.
echo =========================================================
echo   Sales Quotation API - Database Setup
echo =========================================================
echo.

REM Set colors
color 0A

REM Check if running from correct directory
if not exist "SalesQuotation.API\" (
    echo ERROR: Please run this script from the project root directory
    echo Expected to find: SalesQuotation.API folder
    pause
    exit /b 1
)

echo [1/5] Checking .NET tools...
dotnet tool list --global | find "dotnet-ef" >nul
if errorlevel 1 (
    echo Installing dotnet-ef tool...
    dotnet tool install --global dotnet-ef
    if errorlevel 1 (
        echo ERROR: Failed to install dotnet-ef
        pause
        exit /b 1
    )
) else (
    echo ✓ dotnet-ef already installed
)

echo.
echo [2/5] Navigating to API project...
cd SalesQuotation.API
if errorlevel 1 (
    echo ERROR: Could not navigate to SalesQuotation.API
    pause
    exit /b 1
)
echo ✓ In API project directory

echo.
echo [3/5] Checking database connection...
REM Try to list migrations (will fail if DB doesn't exist, which is expected)
dotnet ef migrations list >nul 2>&1
if errorlevel 1 (
    echo ✓ Database does not exist (normal for first run)
) else (
    echo ✓ Database connection successful
)

echo.
echo [4/5] Creating initial migration...
dotnet ef migrations add InitialCreate --project ../SalesQuotation.Infrastructure
if errorlevel 1 (
    echo.
    echo Note: Migration may already exist (this is normal)
) else (
    echo ✓ Migration created successfully
)

echo.
echo [5/5] Creating database...
dotnet ef database update
if errorlevel 1 (
    echo ERROR: Failed to create database
    pause
    exit /b 1
)
echo ✓ Database created successfully

echo.
echo =========================================================
echo   ✓ Database Setup Complete!
echo =========================================================
echo.
echo Next steps:
echo   1. Update JWT secret in appsettings.json
echo   2. Run: dotnet run
echo   3. Test at: http://localhost:5000/swagger
echo.
pause
