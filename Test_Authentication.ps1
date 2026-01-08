# FinovaERP Authentication System Test Script
Write-Host ""🧪 Testing FinovaERP Authentication System"" -ForegroundColor Green

# Test database connection
Write-Host ""
🔍 Testing Database Connection..."" -ForegroundColor Yellow
try {
    $dbContext = Get-Service FinovaDbContext
    $canConnect = await $dbContext.Database.CanConnectAsync()
    if ($canConnect) {
        Write-Host ""✅ Database connection successful"" -ForegroundColor Green
    } else {
        Write-Host ""❌ Cannot connect to database"" -ForegroundColor Red
    }
} catch {
    Write-Host ""❌ Database connection error: $($_.Exception.Message)"" -ForegroundColor Red
}

# Test password hashing
Write-Host ""
🔐 Testing Password Hashing..."" -ForegroundColor Yellow
try {
    $passwordHasher = Get-Service IPasswordHasher
    
    $testPassword = ""Test123!""
    $hashedPassword = $passwordHasher.HashPassword($testPassword)
    
    Write-Host ""Original Password: $testPassword"" -ForegroundColor White
    Write-Host ""Hashed Password: $($hashedPassword.Substring(0, 20))..."" -ForegroundColor Gray
    
    $isValid = $passwordHasher.VerifyPassword($testPassword, $hashedPassword)
    Write-Host ""Password Verification: $($isValid ? '✅ Valid' : '❌ Invalid')"" -ForegroundColor $($isValid ? 'Green' : 'Red')
    
    $isStrong = $passwordHasher.IsPasswordStrong($testPassword)
    Write-Host ""Password Strength: $($isStrong ? '✅ Strong' : '⚠️ Weak')"" -ForegroundColor $($isStrong ? 'Green' : 'Yellow'')
} catch {
    Write-Host ""❌ Password hashing error: $($_.Exception.Message)"" -ForegroundColor Red
}

Write-Host ""
✅ Authentication system test completed"" -ForegroundColor Green
