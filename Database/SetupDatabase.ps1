# FinovaERP Database Setup Script
# Run this script to create and initialize the database
param(
[string]$ServerName = "localhost\SQLEXPRESS",
[string]$DatabaseName = "FinovaERP",
[string]$ScriptPath = "D:\Porjects\FinovaERP\Database\CreateDatabase.sql"
)
Write-Host "Setting up FinovaERP Database..." -ForegroundColor Cyan
try {
# Load SQL Server module
if (-not (Get-Module -Name SqlServer -ListAvailable)) {
Write-Host "Installing SqlServer module..." -ForegroundColor Yellow
Install-Module -Name SqlServer -Force -AllowClobber -Scope CurrentUser
}
Import-Module SqlServer -Force
# Read and execute SQL script
Write-Host "Reading SQL script..." -ForegroundColor Yellow
$sqlScript = Get-Content -Path $ScriptPath -Raw
Write-Host "Executing database creation script..." -ForegroundColor Yellow
Invoke-Sqlcmd -ServerInstance $ServerName -Database "master" -Query $sqlScript -QueryTimeout 300
Write-Host "✅ Database created successfully!" -ForegroundColor Green
# Test connection
Write-Host "Testing database connection..." -ForegroundColor Yellow
$testQuery = "SELECT COUNT(*) as Count FROM sysobjects WHERE type='U'"
$result = Invoke-Sqlcmd -ServerInstance $ServerName -Database $DatabaseName -Query $testQuery
Write-Host "✅ Database connection test passed!" -ForegroundColor Green
Write-Host "Number of tables created: $($result.Count)" -ForegroundColor Cyan
} catch {
Write-Host "❌ Error setting up database: $($_.Exception.Message)" -ForegroundColor Red
exit 1
}
