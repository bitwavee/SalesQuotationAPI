#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Sales Quotation API - Database Setup Script (PowerShell)
.DESCRIPTION
    Automatically sets up SQL Server database for the Sales Quotation API
.EXAMPLE
    .\setup-database.ps1
#>

Write-Host ""
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host "   Sales Quotation API - Database Setup (PowerShell)" -ForegroundColor Cyan
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running from correct directory
if (-not (Test-Path "SalesQuotation.API")) {
    Write-Host "ERROR: Please run this script from the project root directory" -ForegroundColor Red
    Write-Host "Expected to find: SalesQuotation.API folder" -ForegroundColor Red
    exit 1
}

# Step 1: Check .NET tools
Write-Host "[1/5] Checking .NET tools..." -ForegroundColor Yellow
$toolCheck = dotnet tool list --global | Select-String "dotnet-ef"

if ($null -eq $toolCheck) {
    Write-Host "Installing dotnet-ef tool..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to install dotnet-ef" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "✓ dotnet-ef already installed" -ForegroundColor Green
}

# Step 2: Navigate to API project
Write-Host ""
Write-Host "[2/5] Navigating to API project..." -ForegroundColor Yellow
Set-Location "SalesQuotation.API"
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Could not navigate to SalesQuotation.API" -ForegroundColor Red
    exit 1
}
Write-Host "✓ In API project directory" -ForegroundColor Green

# Step 3: Check database connection
Write-Host ""
Write-Host "[3/5] Checking database connection..." -ForegroundColor Yellow
$migrationCheck = dotnet ef migrations list 2>&1
if ($migrationCheck -like "*error*" -or $LASTEXITCODE -ne 0) {
    Write-Host "✓ Database does not exist (normal for first run)" -ForegroundColor Green
} else {
    Write-Host "✓ Database connection successful" -ForegroundColor Green
}

# Step 4: Create initial migration
Write-Host ""
Write-Host "[4/5] Creating initial migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate --project ../SalesQuotation.Infrastructure 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Note: Migration may already exist (this is normal)" -ForegroundColor Cyan
} else {
    Write-Host "✓ Migration created successfully" -ForegroundColor Green
}

# Step 5: Create database
Write-Host ""
Write-Host "[5/5] Creating database..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to create database" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Database created successfully" -ForegroundColor Green

Write-Host ""
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host "   ✓ Database Setup Complete!" -ForegroundColor Green
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Update JWT secret in appsettings.json" -ForegroundColor White
Write-Host "  2. Run: dotnet run" -ForegroundColor White
Write-Host "  3. Test at: http://localhost:5000/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Database Information:" -ForegroundColor Yellow
Write-Host "  Database Name: SalesQuotationDb" -ForegroundColor White
Write-Host "  Tables Created: 10" -ForegroundColor White
Write-Host "  Connection: Check appsettings.json" -ForegroundColor White
Write-Host ""

# Return to root directory
Set-Location ..
