$connString = 'Server=tcp:lunawash-server-db.database.windows.net,1433;Initial Catalog=LunaWashDB;Persist Security Info=False;User ID=LunaWashAdmin;Password=Lun@WashAdmin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = 'SELECT CustomerId, COUNT(*) FROM CustomerVehicles GROUP BY CustomerId HAVING COUNT(*) > 1'
$reader = $command.ExecuteReader()
$hasDuplicates = $false
while ($reader.Read()) {
    $hasDuplicates = $true
    Write-Output $reader[0].ToString()
}
if (-not $hasDuplicates) {
    Write-Output 'No duplicates'
}
$connection.Close()
