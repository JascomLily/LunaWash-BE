import pyodbc

conn_str = 'Driver={ODBC Driver 17 for SQL Server};Server=tcp:lunawash-server-db.database.windows.net,1433;Database=LunaWash;Uid=LunaWashAdmin;Pwd=S@nsnope281121;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;'
try:
    conn = pyodbc.connect(conn_str)
    print('Connected successfully to LunaWash')
    conn.close()
except Exception as e:
    print('Failed with LunaWash database. Exception:', e)

conn_str2 = 'Driver={ODBC Driver 17 for SQL Server};Server=tcp:lunawash-server-db.database.windows.net,1433;Database=Car_Wash_System_DB;Uid=LunaWashAdmin;Pwd=S@nsnope281121;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;'
try:
    conn = pyodbc.connect(conn_str2)
    print('Connected successfully to Car_Wash_System_DB')
    conn.close()
except Exception as e:
    print('Failed with Car_Wash_System_DB database. Exception:', e)
