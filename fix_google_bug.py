import re

path = r'd:\SWP\LunaWash-BE\LunaWash.BLL\Services\AuthService.cs'
with open(path, 'r', encoding='utf-8') as f:
    content = f.read()

# Fix existingUser.Role being null for new users
content = content.replace('if (existingUser.Role.RoleName == "Customer")', 'if (existingUser.Role?.RoleName == "Customer" || existingUser.RoleId == "ROLE-CUS")')

with open(path, 'w', encoding='utf-8') as f:
    f.write(content)
print('Fixed NullReferenceException in GoogleLoginAsync!')
