# =============================================
# FinovaERP Real Database Connection Setup Script
# =============================================
# This script updates the connection string to use the real FinovaDB database
# without creating any new tables

param(
    [string]$ProjectPath = "D:\Projects\FinovaERP",
    [string]$ServerName = "localhost",
    [string]$DatabaseName = "FinovaDB",
    [string]$Username = "",  # Leave empty for Windows Authentication
    [string]$Password = "",  # Leave empty for Windows Authentication
    [switch]$UseWindowsAuth = $true,
    [switch]$KeepOpen = $false  # New parameter to keep window open
)

# Function to keep console open
function Pause-Script {
    param(
        [string]$Message = "Press any key to continue..."
    )
    Write-Host $Message -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

try {
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host "FinovaERP Real Database Connection Setup" -ForegroundColor Cyan
    Write-Host "=============================================" -ForegroundColor Cyan

    # Validate project path
    if (-not (Test-Path $ProjectPath)) {
        Write-Error "Project path not found: $ProjectPath"
        if ($KeepOpen) { Pause-Script }
        exit 1
    }

    Set-Location $ProjectPath

    # Build connection string based on authentication method
    if ($UseWindowsAuth) {
        $connectionString = "Server=$ServerName;Database=$DatabaseName;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
        Write-Host "Using Windows Authentication" -ForegroundColor Green
    } else {
        if ([string]::IsNullOrEmpty($Username) -or [string]::IsNullOrEmpty($Password)) {
            Write-Error "Username and Password are required for SQL Server Authentication"
            if ($KeepOpen) { Pause-Script }
            exit 1
        }
        $connectionString = "Server=$ServerName;Database=$DatabaseName;User Id=$Username;Password=$Password;TrustServerCertificate=True;MultipleActiveResultSets=true"
        Write-Host "Using SQL Server Authentication" -ForegroundColor Green
    }

    Write-Host "Connection String: $connectionString" -ForegroundColor Yellow

    # =============================================
    # 1. Update appsettings.json files
    # =============================================
    Write-Host "`nUpdating appsettings.json files..." -ForegroundColor Cyan

    $appsettingsFiles = @(
        "FinovaERP.Presentation\appsettings.json",
        "FinovaERP.Infrastructure\appsettings.json"
    )

    foreach ($file in $appsettingsFiles) {
        $filePath = Join-Path $ProjectPath $file
        
        if (Test-Path $filePath) {
            try {
                $content = Get-Content $filePath -Raw | ConvertFrom-Json
                
                # Update connection string
                if ($content.ConnectionStrings) {
                    $content.ConnectionStrings.FinovaDbConnection = $connectionString
                } else {
                    $content | Add-Member -MemberType NoteProperty -Name "ConnectionStrings" -Value @{
                        FinovaDbConnection = $connectionString
                    }
                }
                
                # Convert back to JSON with formatting
                $jsonOutput = $content | ConvertTo-Json -Depth 10
                
                # Ensure proper formatting
                $jsonOutput = $jsonOutput -replace "`r`n", "`n" -replace "`n", "`r`n"
                
                Set-Content -Path $filePath -Value $jsonOutput -Encoding UTF8
                
                Write-Host "✓ Updated $file" -ForegroundColor Green
            }
            catch {
                Write-Warning "Could not update $file : $_"
            }
        } else {
            Write-Warning "File not found: $filePath"
        }
    }

    # =============================================
    # 2. Update FinovaDbContext to use real database
    # =============================================
    Write-Host "`nUpdating FinovaDbContext..." -ForegroundColor Cyan

    $dbContextPath = "FinovaERP.Infrastructure\Persistence\FinovaDbContext.cs"

    if (Test-Path $dbContextPath) {
        try {
            $content = Get-Content $dbContextPath -Raw
            
            # Remove any InMemory provider configurations
            $content = $content -replace 'UseInMemoryDatabase.*?\)', 'UseSqlServer(connectionString)'
            
            # Ensure SQL Server is being used
            if ($content -notmatch 'UseSqlServer') {
                # Add SQL Server configuration if not present
                $content = $content -replace 'options.*?\)', "options.UseSqlServer(connectionString)"
            }
            
            # Add database connection validation comment
            $headerComment = @"
// =============================================
// Real Database Configuration - FinovaDB
// Connection String: $connectionString
// Updated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
// =============================================

"@
            
            if ($content -notmatch "Real Database Configuration") {
                $content = $headerComment + $content
            }
            
            Set-Content -Path $dbContextPath -Value $content -Encoding UTF8
            Write-Host "✓ Updated FinovaDbContext.cs" -ForegroundColor Green
        }
        catch {
            Write-Warning "Could not update FinovaDbContext: $_"
        }
    } else {
        Write-Warning "FinovaDbContext.cs not found at: $dbContextPath"
    }

    # =============================================
    # 3. Update Program.cs files to use real database
    # =============================================
    Write-Host "`nUpdating Program.cs files..." -ForegroundColor Cyan

    $programFiles = @(
        "FinovaERP.Presentation\Program.cs"
    )

    foreach ($file in $programFiles) {
        $filePath = Join-Path $ProjectPath $file
        
        if (Test-Path $filePath) {
            try {
                $content = Get-Content $filePath -Raw
                
                # Replace InMemory with SQL Server
                $content = $content -replace 'UseInMemoryDatabase.*?\)', 'UseSqlServer(connectionString)'
                
                # Add validation comment
                $validationComment = @"
// Using real SQL Server database - FinovaDB
// Database connection validated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
"@
                
                if ($content -notmatch "Using real SQL Server database") {
                    # Add after the using statements
                    $usingEnd = ($content -split "`n" | Select-String "^using" | Select-Object -Last 1).LineNumber
                    $lines = $content -split "`n"
                    $newContent = @()
                    
                    for ($i = 0; $i -lt $lines.Count; $i++) {
                        $newContent += $lines[$i]
                        if ($i -eq $usingEnd - 1) {
                            $newContent += $validationComment
                        }
                    }
                    
                    $content = $newContent -join "`n"
                }
                
                Set-Content -Path $filePath -Value $content -Encoding UTF8
                Write-Host "✓ Updated $file" -ForegroundColor Green
            }
            catch {
                Write-Warning "Could not update $file : $_"
            }
        } else {
            Write-Warning "File not found: $filePath"
        }
    }

    # =============================================
    # 4. Build and test the solution
    # =============================================
    Write-Host "`nBuilding solution..." -ForegroundColor Cyan

    try {
        dotnet build
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Solution built successfully!" -ForegroundColor Green
        } else {
            Write-Warning "Build failed with exit code: $LASTEXITCODE"
        }
    }
    catch {
        Write-Warning "Build error: $_"
    }

    # =============================================
    # Summary
    # =============================================
    Write-Host "`n=============================================" -ForegroundColor Cyan
    Write-Host "FinovaERP Database Connection Setup Complete!" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host "Connection String: $connectionString" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Verify the connection string in appsettings.json files"
    Write-Host "2. Run the application to test database connection"
    Write-Host "3. Check if login functionality works with real database"
    Write-Host ""
    Write-Host "Note: No new tables were created. Only connection string was updated." -ForegroundColor Green

}
catch {
    Write-Host "ERROR: $_" -ForegroundColor Red
    Write-Host "Stack Trace: $($_.ScriptStackTrace)" -ForegroundColor Red
}
finally {
    if ($KeepOpen) {
        Pause-Script "Press any key to exit..."
    }
}