
$connString = "Server=tcp:lunawash-server-db.database.windows.net,1433;Initial Catalog=LunaWashDB;User ID=LunaWashAdmin;Password=Lun@WashAdmin;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "INSERT INTO dbo.Banners (ImageUrl, PlatformType, IsHidden) VALUES ('test', 'Web', 0)"
try { $cmd.ExecuteNonQuery(); Write-Host "Success" } catch { Write-Host $_.Exception.Message }
$conn.Close()

