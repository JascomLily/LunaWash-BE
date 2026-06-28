import json

path = r'd:\SWP\LunaWash-BE\LunaWash.API\appsettings.json'
with open(path, 'r', encoding='utf-8') as f:
    data = json.load(f)

data['Google'] = {
    'ClientId': '897520379970-6qi5jkhmqgnmsisintk6gopj0mi1a6sm.apps.googleusercontent.com'
}

with open(path, 'w', encoding='utf-8') as f:
    json.dump(data, f, indent=2)

print('Updated appsettings.json with Google Client ID!')
