$connString = 'Server=tcp:lunawash-server-db.database.windows.net,1433;Initial Catalog=LunaWashDB;Persist Security Info=False;User ID=LunaWashAdmin;Password=Lun@WashAdmin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = 'SELECT Id, LEN(ImageUrl) FROM Banners'
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "$($reader[0]): $($reader[1])"
}
$connection.Close()
