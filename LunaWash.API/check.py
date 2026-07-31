import pyodbc
import json

conn_str = 'Driver={ODBC Driver 17 for SQL Server};Server=tcp:lunawash-server-db.database.windows.net,1433;Database=LunaWashDB;Uid=LunaWashAdmin;Pwd=S@nsnope281121;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;'
conn = pyodbc.connect(conn_str)
cursor = conn.cursor()

cursor.execute("SELECT Id FROM AspNetUsers WHERE Email='theocean2505@gmail.com'")
row = cursor.fetchone()
if row:
    user_id = row[0]
    print('UserId:', user_id)
    cursor.execute(f"SELECT CurrentPoints, AccumulatedPoints FROM CustomerProfiles WHERE UserId='{user_id}'")
    profile = cursor.fetchone()
    print('Profile Points:', profile)

    cursor.execute(f"SELECT Id, TotalPrice, Notes FROM Bookings WHERE CustomerId='{user_id}'")
    for b in cursor.fetchall():
        print(f"Booking {b.Id}: TotalPrice={b.TotalPrice}")
        print(f"Notes={b.Notes}")
