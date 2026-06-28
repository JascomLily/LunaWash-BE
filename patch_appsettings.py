import json

path = r'd:\SWP\LunaWash-BE\LunaWash.API\appsettings.json'
with open(path, 'r', encoding='utf-8') as f:
    data = json.load(f)

data['ConnectionStrings']['DefaultConnection'] = 'Server=tcp:lunawash-server-db.database.windows.net,1433;Initial Catalog=LunaWashDB;Persist Security Info=False;User ID=LunaWashAdmin;Password=S@nsnope281121;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

with open(path, 'w', encoding='utf-8') as f:
    json.dump(data, f, indent=2)

print('Updated appsettings.json!')
