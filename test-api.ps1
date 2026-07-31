try {
    $response = Invoke-WebRequest -Uri 'http://localhost:5010/api/staff/bookings/history/BRN-Q1-01' -Method Get
    Write-Output "Status: $($response.StatusCode)"
    Write-Output "Content: $($response.Content)"
} catch {
    Write-Output "Error: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        Write-Output "Status: $($_.Exception.Response.StatusCode.value__)"
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        Write-Output "Body: $($reader.ReadToEnd())"
    }
}
