import re

path = r'd:\SWP\LunaWash-BE\LunaWash.BLL\Services\AuthService.cs'
with open(path, 'r', encoding='utf-8') as f:
    content = f.read()

# Revert my previous ugly fix
content = content.replace('if (existingUser.Role?.RoleName == "Customer" || existingUser.RoleId == "ROLE-CUS")', 'if (existingUser.Role.RoleName == "Customer")')

# Add Role = customerRole to user creation
old_user_creation = '''                    existingUser = new User
                    {
                        Id = "USR-" + DateTime.UtcNow.ToString("yyMM") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper(),
                        FullName = payload.Name ?? "Google User",
                        Email = userEmail,
                        RoleId = customerRole.Id,'''

new_user_creation = '''                    existingUser = new User
                    {
                        Id = "USR-" + DateTime.UtcNow.ToString("yyMM") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper(),
                        FullName = payload.Name ?? "Google User",
                        Email = userEmail,
                        RoleId = customerRole.Id,
                        Role = customerRole,'''

content = content.replace(old_user_creation, new_user_creation)

with open(path, 'w', encoding='utf-8') as f:
    f.write(content)
print('Applied correct fix for GoogleLoginAsync!')
