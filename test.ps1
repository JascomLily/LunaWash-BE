$connString = 'Server=tcp:lunawash-server-db.database.windows.net,1433;Initial Catalog=LunaWashDB;Persist Security Info=False;User ID=LunaWashAdmin;Password=Lun@WashAdmin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = 'SELECT COUNT(*) FROM Bookings'
$totalBookings = $command.ExecuteScalar()
$command.CommandText = 'SELECT COUNT(*) FROM Bookings WHERE Status = ''Completed'''
$completedBookings = $command.ExecuteScalar()
$connection.Close()
Write-Output "Total bookings: $totalBookings"
Write-Output "Completed bookings: $completedBookings"
