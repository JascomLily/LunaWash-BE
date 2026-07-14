$connString = 'Server=tcp:lunawash-server-db.database.windows.net,1433;Initial Catalog=LunaWashDB;Persist Security Info=False;User ID=LunaWashAdmin;Password=Lun@WashAdmin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = 'SELECT ScheduledStartTime FROM Bookings WHERE Status = ''Completed'''
$reader = $command.ExecuteReader()
$reader.Read()
$date = $reader.GetDateTime(0)
Write-Output $date.ToString("dd/MM/yyyy")
Write-Output $date.ToString("dd/MM/yyyy", [System.Globalization.CultureInfo]::InvariantCulture)
$connection.Close()
