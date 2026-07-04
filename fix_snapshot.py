import re

path2 = r'd:\SWP\LunaWash-BE\LunaWash.DAL\Migrations\ApplicationDbContextModelSnapshot.cs'
with open(path2, 'r', encoding='utf-8') as f:
    content2 = f.read()

content2 = content2.replace('<<<<<<< HEAD\n', '')
content2 = content2.replace('=======\n', '')
content2 = re.sub(r'>>>>>>> [a-f0-9]+\n', '', content2)

with open(path2, 'w', encoding='utf-8') as f:
    f.write(content2)

