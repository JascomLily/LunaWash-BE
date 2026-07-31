$header = @{
    alg = "HS256"
    typ = "JWT"
} | ConvertTo-Json -Compress

$payload = @{
    sub = "admin"
    jti = [guid]::NewGuid().ToString()
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" = "Admin"
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" = "USR-1111"
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" = "Test Admin"
    exp = [int][double]::Parse((Get-Date (Get-Date).AddHours(1).ToUniversalTime() -UFormat %s))
    iss = "LunaWashAPI"
    aud = "LunaWashApp"
} | ConvertTo-Json -Compress

function Encode-Base64Url($bytes) {
    return [Convert]::ToBase64String($bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')
}

$headerEncoded = Encode-Base64Url([System.Text.Encoding]::UTF8.GetBytes($header))
$payloadEncoded = Encode-Base64Url([System.Text.Encoding]::UTF8.GetBytes($payload))

$dataToSign = "$headerEncoded.$payloadEncoded"

$keyBytes = [System.Text.Encoding]::UTF8.GetBytes("DREAMJOURNEYGOLDENJOURNEY1234567890")
$hmac = New-Object System.Security.Cryptography.HMACSHA256
$hmac.Key = $keyBytes
$signatureBytes = $hmac.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($dataToSign))

$signatureEncoded = Encode-Base64Url($signatureBytes)

$token = "$dataToSign.$signatureEncoded"

Write-Output "TOKEN: $token"

try {
    $response = Invoke-WebRequest -Uri "https://lunawash-be.onrender.com/api/staff/bookings/history/BRN-Q1-01" -Headers @{ Authorization = "Bearer $token" } -Method Get
    Write-Output "STATUS: $($response.StatusCode)"
    Write-Output "CONTENT: $($response.Content)"
} catch {
    Write-Output "ERROR STATUS: $($_.Exception.Response.StatusCode.value__)"
    Write-Output "ERROR CONTENT: $([System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream()).ReadToEnd())"
}
